

using BloodMoon.CustomUI;
using BloodMoon.Repository;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using System.Globalization;


namespace BloodMoon
{
    public partial class CalendarPage : ContentPage
    {
        private DateTime _currentDate = DateTime.Now;
        private readonly CultureInfo _culture = new CultureInfo("ru-RU");
        private DatesHolder _datesHolder;
        private IForecaster _forecaster;
        private bool _firstCreation = true;


        public CalendarPage()
        {
            InitializeComponent();

            _datesHolder =Application.Current.Handler.MauiContext.Services.GetService<DatesHolder>();
            _forecaster = Application.Current.Handler.MauiContext.Services.GetService<IForecaster>();

            SetupDayNames();
        }

        protected override async void OnAppearing()
        {
            if (_firstCreation)
            {
                _firstCreation = false;
                await _datesHolder.PreloadData();
                RefreshCalendar();
            }
            base.OnAppearing();
        }

        private void SetupDayNames()
        {
            DayNamesGrid.Children.Clear();
            // В культуре ru-RU сокращенные имена: вс, пн, вт, ср, чт, пт, сб
            string[] names = _culture.DateTimeFormat.AbbreviatedDayNames;
            // Пересортируем, чтобы начать с Понедельника
            string[] russianOrder = { names[1], names[2], names[3], names[4], names[5], names[6], names[0] };

            for (int i = 0; i < russianOrder.Length; i++)
            {
                DayNamesGrid.Add(new Label
                {
                    Text = russianOrder[i].ToUpper(),
                    HorizontalTextAlignment = TextAlignment.Center,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = (i > 4) ? Colors.Red : Colors.White
                }, i, 0);
            }
        }

        private async void RefreshCalendar()
        {
            await Task.Run(() =>
            {
                var month = _currentDate.ToString("MMMM yyyy", _culture);

                DateTime firstDayOfMonth = new DateTime(_currentDate.Year, _currentDate.Month, 1);
                int offset = ((int)firstDayOfMonth.DayOfWeek + 6) % 7;
                int row = 0;

                List<DayView> dayViews = new();
                int daysInMonth = DateTime.DaysInMonth(_currentDate.Year, _currentDate.Month);
                for (int day = 1; day <= daysInMonth; day++)
                {
                    DateTime thisDate = new DateTime(_currentDate.Year, _currentDate.Month, day);
                    var view = new DayView(
                        thisDate,
                        _datesHolder.ContainsInSelected(thisDate),
                        _datesHolder.ContainsInForecast(thisDate),
                        OnDayClicked);

                    dayViews.Add(view);
                }

                MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    CalendarGrid.Children.Clear();
                    CalendarGrid.RowDefinitions.Clear();
                    MonthYearLabel.Text = month;
                    await Task.Yield();
                    CalendarGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                    //заполнение таблицы по ячееечно
                    int dayNumber = 0;
                    for (int i = 0; i < dayViews.Count; i++)
                    {
                        dayNumber += 1;//порядковый номер дня
                        int col = (dayNumber + offset - 1) % 7;
                        CalendarGrid.Add(dayViews[i], col, row);//вставляем кнопку в конкретную ячейку
                        if (col == 6 && dayNumber < daysInMonth)//нужно ли добавить новую строку для дней
                        {
                            row++;
                            CalendarGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                        }

                        if (dayNumber % 7 == 0)//чтобы ui-поток пачками обновлялся, и не фризился надолго
                            await Task.Yield();
                    }
                    dayViews = null;
                });
            });
        }


        // Метод обработки нажатия
        private void OnDayClicked(object sender, EventArgs e)
        {
            if (sender is DayView view )
            {
                if (_datesHolder.ContainsInSelected(view.Date))
                {
                    // Если дата уже была выбрана — удаляем
                    _datesHolder.RemoveFromSelected(view.Date);
                    SetForecastDays(view.Date, false);
                }
                else
                {
                    // Если новая дата — добавляем
                    _datesHolder.AddToSelected(view.Date);
                    SetForecastDays(view.Date, true);
                }
            }
        }

        private void SetForecastDays(DateTime origDate, bool needAdd)
        {
            var days= _forecaster.CalcForecastDays(origDate);
            foreach(DateTime date in days)
            {
                var targetDay = CalendarGrid.Children
                    .OfType<DayView>()
                    .FirstOrDefault(b => ((DayView)(b.BindingContext)).Date == date);

                if (needAdd)
                {
                    _datesHolder.AddToForecast(date);
                    if (targetDay != null)
                        targetDay.IsForecastDay = true;
                }
                else
                {
                    _datesHolder.RemoveFromForecast(date);
                    if (targetDay != null)
                        targetDay.IsForecastDay = false;
                }
            }
        }

        private void OnPrevMonthClicked(object sender, EventArgs e)
        {
            _currentDate = _currentDate.AddMonths(-1);
            RefreshCalendar();
        }
        private void OnNextMonthClicked(object sender, EventArgs e)
        {
            _currentDate = _currentDate.AddMonths(1);
            RefreshCalendar();
        }
    }
}

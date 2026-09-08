

namespace BloodMoon.CustomUI
{
	public partial class DayView : ContentView
	{
        private static readonly Color _selectedColor = Colors.DarkTurquoise;
        private static readonly Color _forecastColor = Colors.HotPink;
        private static readonly Color _defaultColor = Colors.White;
        private static readonly Color _currentDayColor = Colors.Lime;


        public string Text { get; private set; }
		public DateTime Date { get; private set; }

		private bool _isForecast;
		public bool IsForecastDay 
		{
			get { return _isForecast; }
			set { 
				if(_isForecast!=value)
					_isForecast = value;

				SetDayColor();
			}
			
		}

		private bool _manualSelected;
        private event EventHandler Clicked;

        public DayView(
			DateTime date,
			bool manualSelected,
			bool isForecastDate,
			EventHandler evnt)
		{
			InitializeComponent();

			Date = date;
			Text = date.Date.Day.ToString();
			_manualSelected = manualSelected;
			IsForecastDay = isForecastDate;

			Clicked += evnt;
			DateBtn.Clicked += ResendClick;//привязываем к ккнопке внешний обработчик
			SetDayColor();
			SetVisualStyle();

			BindingContext = this;
		}
		private void ResendClick(object sender, EventArgs e)
		{
			Clicked?.Invoke(this, e);//чтобы наверх клилк уходил  через этот view
		}
        public void SetVisualStyle()
		{
            if (Date.Date == DateTime.Now.Date)//выделение текущей даты
			{
				DateBtn.BorderWidth = 5;
				DateBtn.BorderColor = _currentDayColor;
			}
		}

        private void SelfClickHandler(object sender, EventArgs e)
		{
			_manualSelected = !_manualSelected;
			SetDayColor();
        }


		//ручное выделение всегда перекрывает прогноз
		private void SetDayColor()
		{
			if (_manualSelected)
				DateBtn.Background = _selectedColor;
			else
			{
				if (IsForecastDay)
					DateBtn.Background = _forecastColor;
				else
                    DateBtn.Background = _defaultColor;
            }

		}
    }
}
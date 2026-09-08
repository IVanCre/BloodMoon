


namespace BloodMoon.Repository
{
    public class DatesHolder(IForecaster forecaster)
    {
        IForecaster _forecaster= forecaster;
        private DBManager dbManager = new();
        private readonly HashSet<DateTime> _selectedDates = new();
        private readonly HashSet<DateTime> _forecastDates = new();

        public async Task PreloadData()
        {
            var loaded = await dbManager.GetAllSelectedAsync();
            foreach (var data in loaded)
            {
                _selectedDates.Add(data);

                foreach (var date in _forecaster.CalcForecastDays(data))
                    _forecastDates.Add(date);//сразу считаем прогнозы
            }
        }

        public bool AddToSelected(DateTime date)
        {
            bool result = _selectedDates.Add(date);
            if (result)
                _ = dbManager.AddToSelected(date);

            return result;
        }
        public bool RemoveFromSelected(DateTime date)
        {
            bool result = _selectedDates.Remove(date);
            if (result)
                _ = dbManager.RemoveFromSelected(date);

            return result;
        }
        public bool ContainsInSelected(DateTime date)=>_selectedDates.Contains(date);


        public bool AddToForecast(DateTime date) => _forecastDates.Add(date);
        public bool RemoveFromForecast(DateTime date) => _forecastDates.Remove(date);
        public bool ContainsInForecast(DateTime date) => _forecastDates.Contains(date);
    }
}

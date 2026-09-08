

namespace BloodMoon
{
    internal struct CycleSettings
    {
        public readonly int SyclePeriodDays = 29;
        public readonly int LengthDays = 5;
        public CycleSettings() { }
    }

    public interface IForecaster
    {
        List<DateTime> CalcForecastDays(DateTime origDate);
        DateTime CalcForecast(DateTime origDate);
    }

    internal class ForecastLogic:IForecaster
    {
        private CycleSettings _cycleSettings = new();

        public List<DateTime> CalcForecastDays(DateTime origDate)
        {
            List<DateTime> days = new();
            for (int i = 0; i < _cycleSettings.LengthDays; i++)
            {
                days.Add(origDate.AddDays(_cycleSettings.SyclePeriodDays + i));
            }
            return days;
        }

        public DateTime CalcForecast(DateTime origDate)
        {
            return origDate.AddDays(_cycleSettings.SyclePeriodDays);
        }
    }
}

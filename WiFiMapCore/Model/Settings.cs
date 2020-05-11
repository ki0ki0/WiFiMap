using System;
using System.Configuration;

namespace WiFiMapCore.Model
{
    public static class Settings
    {
        private static readonly Lazy<TimeSpan> PointScanTimeLazy =
            new Lazy<TimeSpan>(GetValueOrDefault("PointScanTime", TimeSpan.FromSeconds(1)));

        private static readonly Lazy<int> PointScanCountLazy = 
            new Lazy<int>(GetValueOrDefault("PointScanCount", 5));

        private static readonly Lazy<TimeSpan> DiagnosticsUpdatePeriodLazy = 
            new Lazy<TimeSpan>(GetValueOrDefault("DiagnosticsUpdatePeriod", TimeSpan.FromSeconds(1)));

        public static TimeSpan PointScanTime => PointScanTimeLazy.Value;
        public static int PointScanCount => PointScanCountLazy.Value;
        public static TimeSpan DiagnosticsUpdatePeriod => DiagnosticsUpdatePeriodLazy.Value;
        
        private static T GetValueOrDefault<T>(string name, T defaultValue = default)
        {
            var value = ConfigurationManager.AppSettings.Get(name);
            switch (defaultValue)
            {
                case TimeSpan _:
                    return TimeSpan.TryParse(value, out var timeSpan) ? (T) (object) timeSpan : defaultValue;
                case int _:
                    return int.TryParse(value, out var i) ? (T) (object) i : defaultValue;
            }

            return defaultValue;
        }
    }
}
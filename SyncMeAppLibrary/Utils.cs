using NLog;
using static SyncMeAppLibrary.Model.InputParameters;

namespace SyncMeAppLibrary
{
    public class Utils
    {
        public static string GetSubstring(string[] args, string parameterName)
        {
            var searchParameter = $"{parameterName}=";
            string foundParameter = string.Empty;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith(searchParameter, StringComparison.CurrentCultureIgnoreCase))
                {
                    foundParameter = args[i].Substring(searchParameter.Length);
                }
            }
            if (string.IsNullOrEmpty(foundParameter) && !Enum.IsDefined(typeof(consoleParameters), parameterName))
            {
                throw new Exception($"Unknown input parameter {parameterName}!");
            }
            return foundParameter;
        }

        public static LogLevel ConvertStringToLogLevel(string logLevel)
        {
            switch (logLevel)
            {
                case "Debug":
                    return LogLevel.Debug;
                case "Info":
                    return LogLevel.Info;
                case "Warn":
                    return LogLevel.Warn;
                case "Error":
                    return LogLevel.Error;
                case "Fatal":
                    return LogLevel.Fatal;
                case "Off":
                    return LogLevel.Off;
                default:
                    throw new Exception($"Unknown LogLevel parameter {logLevel}!");
            }

        }

        public static double ConvertToMiliseconds(double value, string unit)
        {
            switch (unit.ToLower())
            {
                case "ms":
                    return value;
                case "s":
                    return value * 1000;
                case "min":
                    return value * 60000;
                case "h":
                    return value * 3600000;
                case "d":
                    return value * 86400000;
                default:
                    throw new Exception($"Time unit {unit} not recognized!");
            }
        }
    }
}

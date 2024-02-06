using NLog;
using static SyncMeAppLibrary.Model.InputParameters;

namespace SyncMeAppLibrary
{
    public class Utils
    {
        /// <summary>
        /// If parameter is found, returns its value (given by 'parameter=''value'). 
        /// </summary>
        /// <param name="args">Array of parameters</param>
        /// <param name="parameterName">Parameter name including '=' sign</param>
        /// <returns>Parameter value</returns>
        /// <exception cref="Exception"></exception>
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

        /// <summary>
        /// Converts string value of parameter to LogLevel property.
        /// </summary>
        /// <param name="logLevel">String value of log level</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static LogLevel ConvertStringToLogLevel(string logLevel)
        {
            switch (logLevel.ToLower())
            {
                case "debug":
                    return LogLevel.Debug;
                case "info":
                    return LogLevel.Info;
                case "warn":
                    return LogLevel.Warn;
                case "error":
                    return LogLevel.Error;
                case "fatal":
                    return LogLevel.Fatal;
                case "off":
                    return LogLevel.Off;
                default:
                    throw new Exception($"Unknown LogLevel parameter {logLevel}!");
            }

        }

        /// <summary>
        /// Converts value to ms based on input time unit.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        /// <returns>number of miliseconds</returns>
        /// <exception cref="Exception"></exception>
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

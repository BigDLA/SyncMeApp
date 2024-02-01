using NLog;
using SyncMeAppLibrary.Model;
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

        // testovací metoda - smazat
        public static void ListInputParameters(InputParameters inputParameters)
        {
            Console.WriteLine($"SourceDirectory = {inputParameters.SourceDirectory}"); 
            Console.WriteLine($"ReplicaDirectory = {inputParameters.ReplicaDirectory}");
            Console.WriteLine($"LogFile = {inputParameters.LogFile}");
            Console.WriteLine($"Interval = {inputParameters.Interval}");
            Console.WriteLine($"TimeUnit = {inputParameters.TimeUnit}");
            Console.WriteLine($"Force = {inputParameters.Force}");
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

        public static bool ConfirmationDialog(string message, bool showDialog = true)
        {
            if (!showDialog)
                return true;

            Console.WriteLine(message);
            if (Console.ReadKey().Key == ConsoleKey.Y)
                return true;
            
            return false;
        }
    }
}

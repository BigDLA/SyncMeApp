using NLog;
using SyncMeAppLibrary.Model;

namespace SyncMeAppLibrary
{
    public class Utils
    {
        public static string GetSubstring(string[] args, string parameterName)
        {
            var searchParameter = $"{parameterName}=";
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith(searchParameter, StringComparison.CurrentCultureIgnoreCase))
                {
                    return args[i].Substring(searchParameter.Length);
                }
            }
            throw new Exception($"Unknown input parameter {parameterName}!");
        }

        // testovací metoda - smazat
        public static void ListInputParameters(InputParameters inputParameters)
        {
            Console.WriteLine($"SourceFolder = {inputParameters.SourceFolder}"); 
            Console.WriteLine($"ReplicaFolder = {inputParameters.ReplicaFolder}");
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

        public static bool ConfirmationDialog(string message)
        {
            Console.WriteLine(message);
            if (Console.ReadKey().Key == ConsoleKey.Y)
                return true;
            
            return false;
        }
    }
}

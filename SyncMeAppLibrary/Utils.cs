using NLog;
using SyncMeAppLibrary.BL;
using SyncMeAppLibrary.Model;
using System.Timers;
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

        public static bool ConfirmationDialog(string message, bool showDialog = true)
        {
            if (!showDialog)
                return true;

            Console.WriteLine(message);
            if (Console.ReadKey().Key == ConsoleKey.Y)
                return true;

            return false;
        }

        public static string[] RelativePaths(DirectoryInfo[] directories, string rootDirectory)
        {
            List<string> relativePaths = new List<string> { };
            foreach (DirectoryInfo dir in directories)
            {
                if (!dir.FullName.Equals(rootDirectory, StringComparison.OrdinalIgnoreCase)) // Root directory will be skiped.
                    relativePaths.Add(dir.FullName.Replace(rootDirectory, string.Empty));
            }
            return relativePaths.ToArray();
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
       /* public static bool TryReadLine(out string line, int timeOutMillisecs = Timeout.Infinite)
        {
            getInput.Set();
            bool success = gotInput.WaitOne(timeOutMillisecs);
            if (success)
                line = input;
            else
                line = null;
            return success;
        }*/
    }
}

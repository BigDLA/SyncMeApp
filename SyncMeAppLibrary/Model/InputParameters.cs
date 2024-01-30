using NLog;
using System.Runtime.CompilerServices;

namespace SyncMeAppLibrary.Model
{
    public class InputParameters
    {
        public enum consoleParameters { sourcefolder, replicafolder, logFile, interval, timeunit, force, fileloglevel, consoleloglevel };

        public string SourceFolder { get; set; }
        public string ReplicaFolder { get; set; }
        public string LogFile { get; set; }
        public long Interval { get; set; }
        public string TimeUnit { get; set; }
        public bool Force { get; set; }
        public string FileLogLevel { get; set; }
        public string ConsoleLogLevel { get; set; }


        public InputParameters(string[] args)
        {
            SourceFolder = Utils.GetSubstring(args, consoleParameters.sourcefolder.ToString()).Replace("'", string.Empty);
            ReplicaFolder = Utils.GetSubstring(args, consoleParameters.replicafolder.ToString()).Replace("'", string.Empty);
            LogFile = Utils.GetSubstring(args, consoleParameters.logFile.ToString()).Replace("'", string.Empty);
            Interval = long.Parse(Utils.GetSubstring(args, consoleParameters.interval.ToString()));
            TimeUnit = Utils.GetSubstring(args, consoleParameters.timeunit.ToString());

            Force = false;
            string force = Utils.GetSubstring(args, consoleParameters.force.ToString());
            if (!string.IsNullOrEmpty(force))
            {
                Force = bool.Parse(force);
            }

            FileLogLevel = "Warn";
            string fileLogLevel = Utils.GetSubstring(args, consoleParameters.fileloglevel.ToString());
            if (!string.IsNullOrEmpty(fileLogLevel))
            {
                FileLogLevel = fileLogLevel;
            }

            ConsoleLogLevel = "Info";
            string consoleLogLevel = Utils.GetSubstring(args, consoleParameters.consoleloglevel.ToString());
            if (!string.IsNullOrEmpty(consoleLogLevel))
            {
                ConsoleLogLevel = consoleLogLevel;
            }
        }

    }
}

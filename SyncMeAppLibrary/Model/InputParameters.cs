﻿namespace SyncMeAppLibrary.Model
{
    public class InputParameters
    {
        public enum consoleParameters { sourcedirectory, replicadirectory, logFile, interval, timeunit, force, fileloglevel, consoleloglevel, askdeleteconfirmation };

        public string SourceDirectory { get; set; }
        public string ReplicaDirectory { get; set; }
        public string LogFile { get; set; }
        public double Interval { get; set; }
        public string? TimeUnit { get; set; }
        public string FileLogLevel { get; set; }
        public string ConsoleLogLevel { get; set; }


        public InputParameters(string[] args)
        {
            SourceDirectory = Utils.GetSubstring(args, consoleParameters.sourcedirectory.ToString());
            ReplicaDirectory = Utils.GetSubstring(args, consoleParameters.replicadirectory.ToString());
            LogFile = Utils.GetSubstring(args, consoleParameters.logFile.ToString());

            Interval = 0;
            string interval = Utils.GetSubstring(args, consoleParameters.interval.ToString());
            if (!string.IsNullOrEmpty(interval))
            {
                Interval = long.Parse(interval);
            }

            TimeUnit = Utils.GetSubstring(args, consoleParameters.timeunit.ToString());

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

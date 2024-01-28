using System.Reflection.Metadata.Ecma335;

namespace SyncMeAppLibrary
{
    public class InputParameters
    {
        public enum consoleParameters {sourcefolder, replicafolder, logfolder, interval, timeunit, force};

        public string SourceFolder { get; set; }
        public string ReplicaFolder { get; set; }
        public string LogFolder { get; set; }
        public long Interval { get; set; }
        public string TimeUnit { get; set; }
        public bool Force { get; set; } // To-DO: jak ho udělat volitelný?

        public InputParameters(string[] args) 
        { 
            SourceFolder = Utils.GetSubstring(args, consoleParameters.sourcefolder.ToString()).Replace("'",string.Empty);
            ReplicaFolder = Utils.GetSubstring(args, consoleParameters.replicafolder.ToString()).Replace("'", string.Empty);
            LogFolder = Utils.GetSubstring(args, consoleParameters.logfolder.ToString()).Replace("'", string.Empty);
            Interval = long.Parse(Utils.GetSubstring(args, consoleParameters.interval.ToString()));
            TimeUnit = Utils.GetSubstring(args, consoleParameters.timeunit.ToString());
            Force = bool.Parse(Utils.GetSubstring(args,consoleParameters.force.ToString()));
        }


    }
}

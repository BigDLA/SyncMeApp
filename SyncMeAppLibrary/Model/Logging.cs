using NLog;

namespace SyncMeAppLibrary.Model
{
    public class Logging
    {
        public static Logger ConfigureLogging(InputParameters inputParameters)
        {
            var config = new NLog.Config.LoggingConfiguration();

            // Targets where to log to: File and Console
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = inputParameters.LogFile };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            // Rules for mapping loggers to targets            
            config.AddRule(Utils.ConvertStringToLogLevel(inputParameters.ConsoleLogLevel), LogLevel.Fatal, logconsole);
            config.AddRule(Utils.ConvertStringToLogLevel(inputParameters.FileLogLevel), LogLevel.Fatal, logfile);

            // Apply config           
            LogManager.Configuration = config;
            return LogManager.GetCurrentClassLogger();
        }

        public const string msgSyncCheck = "Checking that folders are synchronized.";
        public const string msgSyncSuccesfull = "Synchronization finished succesfully!";
        public const string msgDeleteConfirmation = "Target folder is not empty! Delete files in target directory? Y/N:";
        public const string msgActionCanceledByUser = "Action canceld by user!";
    }
}

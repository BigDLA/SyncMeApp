using SyncMeAppLibrary;
using SyncMeAppLibrary.BL;
using SyncMeAppLibrary.Model;
using System.Diagnostics;

namespace SyncMeApp
{
    public class App
    {
        /*private readonly IMessages _messages;

        public App(IMessages messages)
        {
            _messages = messages;
        }*/

        public void Run(string[] args)
        {
            Debugger.Launch();
            var inputParameters = new InputParameters(args);
            var log = Logging.ConfigureLogging(inputParameters); // Initialize and set logging configuration
            
            Utils.ListInputParameters(inputParameters); // pro debug - smazat
            
            SyncLogic.ReplicateSourceDirectory(inputParameters, log);
            log.Info(Logging.msgSyncSuccesfull);
        }
    }
}

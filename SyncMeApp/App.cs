using SyncMeAppLibrary;
using SyncMeAppLibrary.BL;
using SyncMeAppLibrary.Model;

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
            var inputParameters = new InputParameters(args);
            var log = Logging.ConfigureLogging(inputParameters); // Initialize and set logging configuration
            
            Utils.ListInputParameters(inputParameters); // pro debug - smazat
            
            SyncLogic.ReplicateSourceFolder(inputParameters, log);
            log.Info(Logging.msgSyncSuccesfull);
        }
    }
}

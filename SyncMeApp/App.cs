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
            Utils.ListInputParameters(inputParameters); // pro debug - smazat

            SyncLogic.ReplicateSourceFolder(inputParameters);


            //string message = _messages.Greeting(lang);

            //Console.WriteLine(message);
        }
    }
}

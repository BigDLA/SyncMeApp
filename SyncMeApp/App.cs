using System;
using System.Text.RegularExpressions;
using SyncMeAppLibrary;

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


           
            //string message = _messages.Greeting(lang);

            //Console.WriteLine(message);
        }
    }
}

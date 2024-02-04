﻿using SyncMeAppLibrary;
using SyncMeAppLibrary.BL;
using SyncMeAppLibrary.Model;
using System.Diagnostics;

namespace SyncMeApp
{
    public class App
    {
        public async void Run(string[] args)
        {
            Debugger.Launch();
            var inputParameters = new InputParameters(args);
            var log = Logging.ConfigureLogging(inputParameters); // Initialize and set logging configuration
            SyncLogic.ReplicateSourceDirectory(inputParameters, log);
            log.Info(Logging.msgSyncSuccesfull);

            if (inputParameters.Interval > 0)
            {
                var unit = "ms";
                if (!string.IsNullOrEmpty(inputParameters.TimeUnit))
                    unit = inputParameters.TimeUnit;

                log.Info($"Setting timer for {inputParameters.Interval} {unit}");
                using PeriodicTimer timer = new(TimeSpan.FromMilliseconds(Utils.ConvertToMiliseconds(inputParameters.Interval, unit)));
                var msgTimer = $"Waiting for the timer set to {inputParameters.Interval} {unit}. To exit the application press 'Q'.";

                log.Info(msgTimer);
                while (await timer.WaitForNextTickAsync() && !(Console.ReadKey().Key == ConsoleKey.Q))
                {
                    log.Info("Timer has fired.");
                    SyncLogic.ReplicateSourceDirectory(inputParameters, log);
                    log.Info(msgTimer);
                }
                log.Info("Timer cancelled. Exitting the application");
            }
            else
            {
                log.Info("Timer not set. Exitting the application");
            }
        }
    }
}

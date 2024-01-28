namespace SyncMeAppLibrary
{
    public class Utils
    {
        public static string GetSubstring(string[] args, string parameterName)
        {
            var searchParameter = $"{parameterName}=";
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith(searchParameter, StringComparison.CurrentCultureIgnoreCase))
                {
                    return args[i].Substring(searchParameter.Length);
                }
            }
            throw new Exception($"Parameter {parameterName} not found!");
        }

        // testovací metoda - smazat
        public static void ListInputParameters(InputParameters inputParameters)
        {
            Console.WriteLine($"SourceFolder = {inputParameters.SourceFolder}"); 
            Console.WriteLine($"ReplicaFolder = {inputParameters.ReplicaFolder}");
            Console.WriteLine($"LogFolder = {inputParameters.LogFolder}");
            Console.WriteLine($"Interval = {inputParameters.Interval}");
            Console.WriteLine($"TimeUnit = {inputParameters.TimeUnit}");
            Console.WriteLine($"Force = {inputParameters.Force}");
        }
    }
}

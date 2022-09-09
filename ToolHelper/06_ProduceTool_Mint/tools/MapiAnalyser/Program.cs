namespace MapiAnalyser
{
    using System;
    using MapiAnalyser.Cache;
    using MapiAnalyser.Execution;
    using Mint.Common;

    class Program
    {
        static void Main(string[] args)
        {
            bool hasArgs = args != null && args.Length > 0;
            string userInput = hasArgs ? args[0] : string.Empty;

            try
            {
                switch (userInput.ToLower())
                {
                    case "cache":
                        MapiCache.CacheAssemblyData();
                        break;
                    case "summary":
                        MapiManager.Summary();
                        break;
                    case "produce":
                        MapiManager.ListProducible();
                        break;
                    case "wave":
                        MapiManager.ListNextWave();
                        break;
                    case "filter":
                        MapiManager.ListAllFilters();
                        break;
                    case "inc":
                        MapiManager.ListIncompatibleAPIs();
                        break;
                    case "api":
                        MapiManager.ListAllIncompatibleAPIs();
                        break;
                    case "find":
                        MapiManager.ListAssembliesWithAPIs();
                        break;
                    case "onetime":
                        OneTime.Run();
                        break;
                    default:
                        ConsoleLog.Message("?");
                        break;
                }
                Console.ResetColor();
            }
            catch (Exception e)
            {
                ConsoleLog.Error(e);
                return;
            }
        }
    }
}

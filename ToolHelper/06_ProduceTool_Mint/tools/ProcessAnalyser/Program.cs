namespace ProcessAnalyser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Mint.Common.Utilities;
    using Mint.Database;

    class Program
    {
        static Dictionary<string, CacheSubFilter> SubCommands = CacheSubFilter.LoadCacheSubFilters();

        static void Main(string[] args)
        {
            UserInputParser.Parse(args, out string process, out string command, out string arg);

            try
            {
                if (Enum.TryParse(process, ignoreCase: true, out Process actualProcess))
                {
                    switch (command)
                    {
                        case "cache":
                            ProcessDataCache.CacheProcessData(actualProcess, arg);
                            break;
                        case "state":
                            AnalyserManager.State(actualProcess);
                            break;
                        case "wave":
                            AnalyserManager.ShowProduceOrder(actualProcess, arg);
                            break;
                        case "pp":
                            ProcessDataCache.FindPerProcessor(actualProcess, arg);
                            break;
                        case "asse":
                            if (string.IsNullOrEmpty(arg))
                            {
                                ConsoleLog.Error("Full dll name.");
                                break;
                            }
                            AnalyserManager.ShowAssemblyDetails(actualProcess, arg);
                            break;
                        default:
                            if (!TryExecuteCustomCommand(actualProcess, command))
                            {
                                ShowUsage();
                            }
                            break;
                    }
                }
                else
                {
                    ConsoleLog.Error($"arg: '{process}' is not a valid process name.");
                    ShowUsage();
                }
                Console.ResetColor();
            }
            catch (Exception e)
            {
                ConsoleLog.Error($"\n{e}");
                return;
            }
        }

        private static bool TryExecuteCustomCommand(Process process, string cmd)
        {
            if (SubCommands.TryGetValue(cmd, out CacheSubFilter? command))
            {
                AnalyserManager.ExecuteCustomCommand(process, command);
                return true;
            }
            return false;
        }

        private static void ShowUsage()
        {
            ConsoleLog.Title("\nMapiHttp Analyser\n");
            ConsoleLog.Highlight("  Buildin Commands:");
            ConsoleLog.Warning("  > process [process] cache   - Cache MapiHttp assemblies data to local files.");
            ConsoleLog.Warning("  > process [process] state   - MapiHttp produce state.");
            ConsoleLog.Warning("  > process [process] wave [cap]  - Show MapiHttp produce order.");
            ConsoleLog.Warning("  > process [process] asse [name] - Search assembly data by full name.");

            if (SubCommands.Any())
            {
                ConsoleLog.Ignore("");
                ConsoleLog.Highlight("  Custom Commands:");
                foreach (var command in SubCommands.Values)
                {
                    ConsoleLog.Warning($"  > process [process] {command.Command,-7} - {command.Description}");
                }
            }
        }
    }
}

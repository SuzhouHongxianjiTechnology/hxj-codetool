namespace ProcessAnalyser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Mint.Common.Extensions;
    using Mint.Common.Utilities;
    using Mint.Database;
    using Mint.DataStructures.DirectedGraph;
    using Mint.Substrate;

    public static class AnalyserManager
    {
        private static LookupTable LookupTable = new LookupTable();


        public static void State(Process process)
        {
            var subCache = ProcessDataCache.ReadSubCache(process);
            // Produced
            int total = subCache.Count;
            int produced = subCache.Where(d => d.IsProduced).Count();
            int canproduce = 0;
            int incompatible = 0;
            int compatible = 0;

            // Incompatible APIs
            var apis = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (SubDllData data in subCache)
            {
                if (!data.IsProduced && !data.BlockedSub.Any() && !data.BlockedNonSub.Any() && !data.IncompatibleAPIs.Any())
                {
                    canproduce++;
                }

                if (!data.IsProduced && (data.IncompatibleAPIs.Any() || data.BlockedNonSub.Any()))
                {
                    apis = apis.Union(data.IncompatibleAPIs).ToHashSet();
                    incompatible++;
                }

                if (!data.IsProduced && !data.BlockedNonSub.Any() && !data.IncompatibleAPIs.Any())
                {
                    compatible++;
                }
            }

            ConsoleLog.Warning($"Produced: {produced}/{total}");
            ConsoleLog.Warning($"Producible: {canproduce}");
            ConsoleLog.Warning($"Compatible: {compatible}");
            ConsoleLog.Warning($"Incompatible: {incompatible}");
            ConsoleLog.Warning($"Incompatible APIs: {apis.Count}");
        }

        public static void ShowProduceOrder(Process process, string waveLimit)
        {
            var subCache = ProcessDataCache.ReadSubCache(process);
            var subMap = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var data in subCache)
            {
                if (!data.BlockedNonSub.Any() & !data.IncompatibleAPIs.Any())
                {
                    subMap.Add(data.AssemblyName);
                }
            }
            var waves = ProduceWaves(process);

            ConsoleLog.Ignore("");
            ConsoleLog.Title("MapiHttp Produce Waves:");
            ConsoleLog.Ignore("");
            ConsoleLog.InLine("[√] Compatible", ConsoleColor.Green);
            ConsoleLog.Message("   - Blocked by Substrate only.");
            ConsoleLog.InLine("[×] Incompatible", ConsoleColor.Red);
            ConsoleLog.Message(" - Blocked by NuGets and/or Imcompatible APIs.");
            ConsoleLog.Ignore("");

            int limit = waves.Count;
            if (Int32.TryParse(waveLimit, out int value))
            {
                limit = Math.Min(value, limit);
                limit = Math.Max(1, limit);
            }
            for (int i = 0; i < limit; i++)
            {
                var wave = waves[i].ToList().OrderBy(a => a);
                ConsoleLog.InLine("---", ConsoleColor.DarkGray);
                ConsoleLog.InLine($" Wave {i + 1}/{waves.Count} ");
                ConsoleLog.Ignore("--------------------------------------------------");
                foreach (var assembly in wave)
                {
                    if (subMap.Contains(assembly))
                    {
                        ConsoleLog.Success($"[√] {assembly}");
                    }
                    else
                    {
                        ConsoleLog.Error($"[×] {assembly}");
                    }
                }
                ConsoleLog.Ignore("");
            }
        }

        public static void ShowAssemblyDetails(Process process, string name)
        {
            var subCache = ProcessDataCache.ReadSubCache(process);
            foreach (var data in subCache)
            {
                if (data.AssemblyName.EqualsIgnoreCase(name))
                {
                    ConsoleLog.Title($"\n{data.AssemblyName} ");
                    ConsoleLog.Path($"{data.FilePath}");
                    ConsoleLog.Ignore("----------------------------------------------------------------");

                    ConsoleLog.InLine("Produced: ");
                    if (data.IsProduced)
                        ConsoleLog.Success("Yes");
                    else
                        ConsoleLog.Error("No");

                    if (data.BlockedSub.Any())
                    {
                        ConsoleLog.Highlight("Blocked by Substrate:");
                        data.BlockedSub.ForEach(b => ConsoleLog.Error($"  - {b}"));
                    }

                    if (data.BlockedNonSub.Any())
                    {
                        ConsoleLog.Highlight("Blocked by NuGet:");
                        data.BlockedNonSub.ForEach(b => ConsoleLog.Error($"  - {b}"));
                    }

                    if (data.IncompatibleAPIs.Any())
                    {
                        ConsoleLog.Highlight("Incompatible APIs: ");
                        data.IncompatibleAPIs.ForEach(a => ConsoleLog.Error($"  - {a}"));
                    }

                    if (data.FilteredAPIs.Any())
                    {
                        ConsoleLog.Highlight("Filtered APIs: ");
                        data.FilteredAPIs.ForEach(a => ConsoleLog.Success($"  - {a}"));
                    }

                }
            }
        }

        public static void ExecuteCustomCommand(Process process, CacheSubFilter filter)
        {
            var subCache = ProcessDataCache.ReadSubCache(process);
            var RPN = CacheSubFilter.ToReversePolishNotation(filter.Conditions);
            var list = subCache.Where(cache => CacheSubFilter.MatchAll(cache, RPN));

            ConsoleLog.Title($"\n{filter.Description}");
            ConsoleLog.InLine("[Condition] ", ConsoleColor.Yellow);
            ConsoleLog.Highlight($"{filter.Conditions}");

            if (list.Any())
            {
                ConsoleLog.Ignore("----------------------------------------------------------------");
                foreach (var data in list)
                {
                    ConsoleLog.Success($"{data.AssemblyName}");
                }
                ConsoleLog.Ignore("----------------------------------------------------------------");
                ConsoleLog.Highlight($"Match: {list.Count()}");
            }
            else
            {
                ConsoleLog.Ignore("----------------------------------------------------------------");
                ConsoleLog.Highlight("No match.");
            }
        }

        private static List<HashSet<string>> ProduceWaves(Process process)
        {
            var subCache = ProcessDataCache.ReadSubCache(process);
            var notProduced = subCache.Where(data => !data.IsProduced);
            var confirm = notProduced.Select(data => data.AssemblyName).ToHashSet();

            var graph = new Graph<string, int>();
            foreach (var v in notProduced)
            {
                graph.AddVertice(v.AssemblyName);
                foreach (var u in v.BlockedSub)
                {
                    if (confirm.Contains(u))
                    {
                        graph.AddEdge(v.AssemblyName, u, 0);
                    }
                }
            }

            var waves = graph.TopologicalWave(out List<Vertice<string>> remain);

            if (remain.Any())
            {
                ConsoleLog.Warning($"\nThere are assemblies can not be resolved due to unknown dependencies:");
                foreach (var v in remain)
                {
                    ConsoleLog.Error($"  {v.Value}");
                    foreach (var left in v.Children)
                    {
                        ConsoleLog.Error($"    - {left}");
                    }
                }
                ConsoleLog.Ignore("----------------------------------------------------------------");
            }
            return waves;
        }

    }
}

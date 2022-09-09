namespace MapiAnalyser.Execution
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using MapiAnalyser.Cache;
    using Mint.Common;
    using Mint.Substrate;
    using Mint.Substrate.Utilities;

    public static class MapiManager
    {
        private static List<AssemblyData> Cache = MapiCache.ReadCache();

        public static void Summary()
        {
            // Produced
            int total = Cache.Count;
            int produced = Cache.Where(d => d.IsProduced).Count();
            int incomp = 0;
            ConsoleLog.Warning($"Produced: {produced}/{total}");

            // Incompatible APIs
            var apis = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var data in Cache)
            {
                if (data.IncompatibleAPIs.Any())
                {
                    apis = apis.Union(data.IncompatibleAPIs).ToHashSet();
                    incomp++;
                }
            }
            ConsoleLog.Warning($"Incompatible Substrate: {incomp}");
            ConsoleLog.Warning($"Incompatible APIs: {apis.Count}");
        }

        public static void ListProducible()
        {
            foreach (var data in Cache)
            {
                if (!data.IsProduced &&
                    !data.BlockedBy.Any() &&
                    !data.IncompatibleAPIs.Any())
                {
                    if (!File.Exists(data.FilePath))
                    {
                        ConsoleLog.Error("Project file in cache but cannot be found anymore, has it been removed?" + Environment.NewLine +
                                     $"(File {data.FilePath})");
                        continue;
                    }
                    if (data.FilePath.EndsWith("csproj"))
                    {
                        ConsoleLog.Success(Environment.NewLine + $"{data.AssemblyName}");
                        ConsoleLog.Path(SubstrateUtils.ToSourcePath(DF.SrcDir, data.FilePath));
                    }
                }
            }
        }

        public static void ListNextWave()
        {
            foreach (var data in Cache)
            {
                if (!data.IsProduced &&
                    !data.BlockedBy.Any() &&
                    data.IncompatibleAPIs.Any())
                {
                    ConsoleLog.Title(Environment.NewLine + $"{data.AssemblyName}");
                    data.IncompatibleAPIs.ForEach(a => ConsoleLog.Error($"  {a}"));
                }
            }
        }

        public static List<string> ListAllFilters()
        {
            var filters = Cache.SelectMany(d => d.FilteredAPIs)
                            .ToHashSet().ToList().OrderBy(a => a).ToList();
            filters.ForEach(a => ConsoleLog.Message(a));
            ConsoleLog.Title($"Total: {filters.Count()}");
            return filters;
        }

        public static void ListIncompatibleAPIs()
        {
            var apis = Cache.SelectMany(d => d.IncompatibleAPIs)
                            .ToHashSet().ToList().OrderBy(a => a).ToList();
            apis.ForEach(a => ConsoleLog.Message(a));
            ConsoleLog.Title($"Total: {apis.Count()}");
        }

        public static void ListAllIncompatibleAPIs()
        {
            var ff = Cache.SelectMany(d => d.FilteredAPIs).ToHashSet();
            var aa = Cache.SelectMany(d => d.IncompatibleAPIs).ToHashSet();
            var all = ff.Union(aa).ToList().OrderBy(a => a).ToList();
            all.ForEach(a => ConsoleLog.Message(a));
            ConsoleLog.Title($"Total: {all.Count()}");
        }

        public static void ListAssembliesWithAPIs()
        {
            var apis = FileUtils.ReadLines(Files.TEMP_APIS);
            foreach (var data in Cache)
            {
                var incAPI = apis.Intersect(data.IncompatibleAPIs);
                var fltAPI = apis.Intersect(data.FilteredAPIs);
                if (incAPI.Any() || fltAPI.Any())
                // if (fltAPI.Any())
                {
                    ConsoleLog.Warning(data.AssemblyName);
                    incAPI.ToList().ForEach(a => ConsoleLog.Error($"    {a}"));
                    fltAPI.ToList().ForEach(a => ConsoleLog.Error($"    {a}"));
                }
            }
        }
    }
}

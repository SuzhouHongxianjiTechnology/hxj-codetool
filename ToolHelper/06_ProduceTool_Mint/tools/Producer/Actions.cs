namespace Producer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Mint.Common.Extensions;
    using Mint.Common.Utilities;
    using Mint.Substrate;
    using Mint.Substrate.Construction;

    internal static class Actions
    {
        private static LookupTable LookupTable = new LookupTable();

        internal static void Produce(string framework)
        {
            VerifyThrowValidNetFrameworkFolder();
            string buildFilePath = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.csproj")[0];

            ConsoleLog.Title("Producing project:");
            ConsoleLog.Path($"{buildFilePath}");
            ConsoleLog.Ignore("----------------------------------------------------------------");

            using (Timer.TimeThis)
            {
                var manager = new ProduceManager(LookupTable, buildFilePath, framework);

                ConsoleLog.LogAction("Reseting restore entry ......... ", () => manager.ResetRestoreEntry());
                ConsoleLog.LogAction("Reseting dirs file ............. ", () => manager.ResetDirsProj());
                ConsoleLog.LogAction("Setting up build file .......... ", () => manager.SetupBuildFile());
                ConsoleLog.LogAction("Converting build file .......... ", () => manager.ConvertBuildFile());
                ConsoleLog.LogAction("Adding path to restore entry ... ", () => manager.AddPathToRestoreEntry());
                ConsoleLog.LogAction("Adding path to dirs file ....... ", () => manager.AddPathToDirsProj());

                AnalyseProducedProject(manager);
            }
        }

        internal static void Move(string component)
        {
            ConsoleLog.Title($"Moving projects under: {component}");
            ConsoleLog.Ignore("----------------------------------------------------------------");
            using (Timer.TimeThis)
            {
                MoveManager.MoveComponents(component);
            }
        }

        internal static void FindAnyString(string keyword)
        {
            var projects = LookupTable.GetProducedProjects();
            foreach (var project in projects)
            {
                int lineNum = 0;
                var matchs = new List<(string, string)>();
                foreach (var line in File.ReadAllLines(project.FilePath))
                {
                    lineNum++;
                    if (line.ContainsIgnoreCase(keyword))
                    {
                        matchs.Add(($"Line: {lineNum,-3}", line));
                    }
                }
                if (matchs.Any())
                {
                    ConsoleLog.Ignore("----------------------------------------------------------------");
                    ConsoleLog.Path(project.FilePath);
                    matchs.ForEach(m =>
                    {
                        ConsoleLog.Success(m.Item1, inLine: true);
                        ConsoleLog.Warning($"  {m.Item2.Trim()}");
                    });
                }
            }
        }

        internal static void GetProducedInfo(string assemblyName)
        {
            var projects = LookupTable.GetProducedProjects();
            bool found = false;
            foreach (var project in projects)
            {
                if (project.Name.ContainsIgnoreCase(assemblyName))
                {
                    found = true;
                    ConsoleLog.Ignore("\n----------------------------------------------------------------");
                    ConsoleLog.Highlight(
                        $"Name      : {project.Name}\n" +
                        $"Type      : {project.Type}\n" +
                        $"Path      : {project.FilePath}\n" +
                        $"Framework : {project.Framework}"
                    );

                    if (!string.IsNullOrEmpty(project.TargetPath))
                    {
                        string absPath = project.TargetPath.ReplaceIgnoreCase("$(TargetPathDir)", Repo.Paths.SrcDir + "\\target\\")
                                                           .ReplaceIgnoreCase("$(FlavorPlatformDir)", "debug\\amd64");
                        ConsoleLog.Highlight($"Target    : {absPath}\n");
                        ConsoleLog.Warning($"<Reference Include=\"{project.Name}\">\n" +
                                           $"  <HintPath>{project.TargetPath}</HintPath>\n" +
                                           $"</Reference>");
                    }
                }
            }
            if (!found)
            {
                ConsoleLog.Error("The specified project could not be found or has not been produced.");
            }
        }

        internal static void GetPackageVersion(string packageName)
        {
            bool found = false;
            ConsoleLog.Highlight($"\nLooking for package: '{packageName}'");
            ConsoleLog.Ignore("----------------------------------------------------------------");

            Package? package;

            var innerPackages = Repo.InnerCorext.GetPackages();
            if (innerPackages.TryGetPackage(packageName, out package))
            {
                found = true;
                ConsoleLog.Warning($"Found in '{Repo.Paths.InnerCorext}', Version: {package.Version}");
            }

            var outerPackages = Repo.OuterCorext.GetPackages();
            if (outerPackages.TryGetPackage(packageName, out package))
            {
                found = true;
                ConsoleLog.Warning($"Found in '{Repo.Paths.OuterCorext}', Version: {package.Version}");
            }

            var propsPackages = Repo.PackagesProps.GetPackages();
            if (propsPackages.TryGetPackage(packageName, out package))
            {
                found = true;
                ConsoleLog.Warning($"Found in '{Repo.Paths.PackagesProps}', Version: {package.Version}");
            }

            if (!found)
            {
                ConsoleLog.Error("The specified package could not be found.");
            }

        }

        internal static void UpdateSubstratePackages()
        {
            var innerPackages = Repo.InnerCorext.GetPackages();
            var outerPackages = Repo.OuterCorext.GetPackages();

            var props = Repo.PackagesProps;
            var packages = props.GetPackages();

            packages.Upgrade(innerPackages, out var innerFails);
            foreach (var fail in innerFails)
            {
                ConsoleLog.Warning($"[SKIP] PKG: {fail.Name}", inLine: true);
                ConsoleLog.Ignore($" {fail.CurrentVersion}  >>  {fail.NewVersion}");
            }

            packages.Upgrade(outerPackages, out var outerFails);
            foreach (var fail in outerFails)
            {
                ConsoleLog.Warning($"[SKIP] PKG: {fail.Name}", inLine: true);
                ConsoleLog.Ignore($" {fail.CurrentVersion}  >>  {fail.NewVersion}");
            }

            props.Save();
        }

        private static void AnalyseProducedProject(ProduceManager manager)
        {
            ConsoleLog.Ignore("----------------------------------------------------------------");
            ConsoleLog.Message("Analyising ...");

            (List<string> blocked, List<string> undefined) result = manager.FindBlockAndUndefined();

            if (result.blocked.Any() || result.undefined.Any())
            {
                ConsoleLog.Error("This project has been produced but with some issues:");
                if (result.blocked.Any())
                {
                    ConsoleLog.Error("* Blocked by Substrate project(s):");
                    result.blocked.ForEach(b => ConsoleLog.Error($"  - {b}"));
                }
                if (result.undefined.Any())
                {
                    ConsoleLog.Error("* Undefined package(s) used:");
                    result.undefined.ForEach(u => ConsoleLog.Error($"  - {u}"));
                }
            }
            else
            {
                ConsoleLog.Success("This project has been successfully produced.");
            }
        }

        private static void VerifyThrowValidNetFrameworkFolder()
        {
            string currentDir = Directory.GetCurrentDirectory();
            if (currentDir.EndsWithIgnoreCase("netcore") ||
                currentDir.EndsWithIgnoreCase("netstd"))
            {
                throw new InvalidOperationException($"Porject has been produced: '{currentDir}'");
            }
            string[] files = Directory.GetFiles(currentDir, "*.csproj");
            if (files.Length != 1)
            {
                throw new InvalidOperationException($"Not a valid project folder: '{currentDir}'");
            }
        }

    }
}

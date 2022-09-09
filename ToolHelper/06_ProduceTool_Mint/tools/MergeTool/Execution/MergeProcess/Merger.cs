namespace MergeTool.Execution
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using MergeTool.Common;
    using Mint.Common;
    using Mint.Substrate;
    using Mint.Substrate.Constants;
    using Mint.Substrate.Construction;
    using Mint.Substrate.Production;
    using Mint.Substrate.Utilities;

    internal static class Merger
    {
        internal static void AutoMerge()
        {
            Timer.Start();

            Git dfGit = new Git(Settings.DFSrc);

            /*
                0. scan all net framework projects from user barnch
            */
            dfGit.Switch(Settings.FixBranch);
            ConsoleLog.Title("Scaning produced projects ... ");
            var configs = Merger.CreatePortingConfigsFromFixBranch();

            /*
                1. checkout to main branch, pull, reset
                   scan all net framework files that have been produced
            */
            ConsoleLog.Ignore("");
            dfGit.ResetBranchTo(Settings.MainBranch, Settings.MainCommitID);

            var filesOnMain = new Dictionary<string, NetFrameworkProjectFile>();
            foreach (var config in configs.ToList())
            {
                string filePath = config.NetFrameworkFilePath;
                if (!File.Exists(filePath))
                {
                    ConsoleLog.Error("Cannot find project file on main branch." + Environment.NewLine +
                                 $"(File {filePath})");
                    configs.Remove(config);
                    continue;
                }
                filesOnMain.Add(filePath, new NetFrameworkProjectFile(filePath));
            }
            ConsoleLog.Message($"{filesOnMain.Count()} projects have been loaded from branch: {Settings.MainBranch}.");

            /*
                2. checkout to sub branch, pull, reset commit id
                   scan all net framework files that have been produced
            */
            ConsoleLog.Ignore("");
            dfGit.ResetBranchTo(Settings.DFBranch, Settings.DFCommitID);

            var filesOnDF = new Dictionary<string, NetFrameworkProjectFile>();
            foreach (var config in configs.ToList())
            {
                string filePath = config.NetFrameworkFilePath;
                if (!File.Exists(filePath))
                {
                    ConsoleLog.Error("File is missing on df branch." + Environment.NewLine +
                                 $"{filePath}");
                    configs.Remove(config);
                    continue;
                }
                filesOnDF.Add(filePath, new NetFrameworkProjectFile(filePath));
            }
            ConsoleLog.Message($"{filesOnDF.Count()} projects have been loaded from branch: {Settings.DFBranch}.");

            /*
                3. checkout to user branch
                   compare those files, apply changes
            */
            ConsoleLog.Ignore("");
            dfGit.Switch(Settings.FixBranch);

            // ================================================================
            // Main compare & apply
            // ================================================================

            ConsoleLog.Title("Comparing projects & apply changes... ");
            var changeList = new List<string>();
            foreach (var config in configs.ToList())
            {
                string filePath = config.NetFrameworkFilePath;
                var mainFile = filesOnMain[filePath];
                var dfFile = filesOnDF[filePath];

                string producedFile = config.ProduceFilePath;
                if (!File.Exists(producedFile))
                {
                    ConsoleLog.Debug("Cannot find produce project file, does its name conform to the specification?" +
                                     Environment.NewLine +
                                     $"(File {producedFile})");
                    continue;
                }

                var changes = mainFile.CompareTo(dfFile, config);
                var result = ApplyChanges(producedFile, changes);
                bool requireFormat = true;

                switch (result)
                {
                    case SyncResult.Succeed:
                        ConsoleLog.Success($"[√] {producedFile}");
                        changeList.Add(mainFile.FullPath);
                        break;
                    case SyncResult.Partially:
                        ConsoleLog.Warning($"[?] {producedFile}");
                        changeList.Add(mainFile.FullPath);
                        break;
                    case SyncResult.Failed:
                        ConsoleLog.Error($"[×] {producedFile}");
                        changeList.Add(mainFile.FullPath);
                        break;
                    case SyncResult.NotChanged:
                    default:
                        requireFormat = false;
                        break;
                }

                if (requireFormat)
                {
                    using (var csproj = new NetCoreProjectFile(producedFile))
                    {
                        csproj.Format();
                    }
                }
            }
            ConsoleLog.Title($"Total changed files: {changeList.Count}");
            File.WriteAllLines(TempFiles.ChangedFiles, changeList);

            // ================================================================

            Timer.Stop();

            // Show commands for checking
            ConsoleLog.Message(Environment.NewLine + "Run this command in enlistment to start merge check:");
            ConsoleLog.Warning(Commands.DiffChangesOnly());
        }

        private static List<PortingConfig> CreatePortingConfigsFromFixBranch()
        {
            var configs = new List<PortingConfig>();
            using (var restoreEntry = DF.RestoreEntry)
            {
                restoreEntry.SortAndRemoveDuplicates();
            }
            var producedFiles = new List<string>();
            var netFrameworkFiles = new List<string>();
            foreach (string producedFile in DF.RestoreEntry.ProjectPaths)
            {
                if (SpecialCases.KnownNetCoreOnlyFiles.Contains(producedFile))
                {
                    continue;
                }

                if (SubstrateUtils.TryFindNetFrameworkFile(producedFile, out string netFrameworkFile))
                {
                    var framework = SubstrateUtils.GetFrameworkByPath(producedFile);
                    if (framework == TargetFramework.NetFramework)
                    {
                        ConsoleLog.Warning("Expecting a NetStd/NetCore file, but get a NetFramework file. Check it in restore entry." + Environment.NewLine +
                                       $"(File {producedFile})");
                        continue;
                    }
                    PortingConfig config = PortingConfig.Create(netFrameworkFile, framework);
                    producedFiles.Add(producedFile);
                    netFrameworkFiles.Add(netFrameworkFile);
                    configs.Add(config);
                }
                else
                {
                    ConsoleLog.Warning("The corresponding NetFramework projcet file is missing, has it been removed from master branch?" + Environment.NewLine +
                                   $"(File {producedFile})");
                }
            }

            File.WriteAllLines(TempFiles.ProducedFiles, producedFiles);
            File.WriteAllLines(TempFiles.NetFrameworkFiles, netFrameworkFiles);

            return configs;
        }

        private static SyncResult ApplyChanges(string producedFilePath, List<BaseDiff> changes)
        {
            int overAllResult = 0;
            if (changes.Any())
            {
                using (var projectFile = new NetCoreProjectFile(producedFilePath))
                {
                    foreach (var change in changes)
                    {
                        var result = change.Apply(projectFile);
                        switch (result)
                        {
                            case SyncResult.Succeed:
                                overAllResult |= 1;
                                break;
                            case SyncResult.Partially:
                                overAllResult |= 2;
                                break;
                            case SyncResult.Failed:
                                overAllResult |= 4;
                                break;
                            case SyncResult.NotChanged:
                            default:
                                break;
                        }
                    }
                }
            }
            return overAllResult >= 4 ? SyncResult.Failed
                                      : overAllResult >= 2 ? SyncResult.Partially
                                                           : overAllResult >= 1 ? SyncResult.Succeed
                                                                                : SyncResult.NotChanged;
        }
    }
}

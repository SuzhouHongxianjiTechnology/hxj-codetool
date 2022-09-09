namespace MergeTool.Execution
{
    using System;
    using System.Linq;
    using Mint.Common;
    using Mint.Substrate;

    internal static class Updater
    {
        internal static void UpdateSubstratePackagesProps()
        {
            ConsoleLog.Title("Upgrade substrate packages ...");
            ConsoleLog.Ignore("----------------------------------------------------------------");
            Timer.Start();
            using (var packageProps = DF.PackagesProps)
            {
                var packages = DF.InnerCorext.Packages.Union(DF.OuterCorext.Packages);
                foreach (var package in packages)
                {
                    try
                    {
                        packageProps.UpgradePackageVersion(package.Key, package.Value);
                    }
                    catch (Exception e)
                    {
                        ConsoleLog.Warning("[skipped] ", inLine: true);
                        ConsoleLog.Message(e.Message);
                    }
                }
            }
            Timer.Stop();

            NuspecUpdater.UpdateHardCodeH();
        }

        internal static void UpdateSubstrateNugetPackages()
        {
            ConsoleLog.Title("Updating nuspec files ...");
            ConsoleLog.Ignore("----------------------------------------------------------------");
            Timer.Start();
            NuspecUpdater.UpdateNuspecFiles();
            Timer.Stop();
        }

        internal static void SortSubstrateRestoreEntry()
        {
            ConsoleLog.Title("Sorting restore entry ...");
            ConsoleLog.Ignore("----------------------------------------------------------------");
            Timer.Start();
            using (var restoreEntry = DF.RestoreEntry)
            {
                restoreEntry.SortAndRemoveDuplicates();
            }
            Timer.Stop();
        }
    }
}

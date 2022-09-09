namespace SyncTool
{
    using System;
    using Mint.Common.Utilities;
    using Mint.Substrate;

    internal static class Actions
    {
        private static LookupTable LookupTable = new LookupTable();
        private static SyncManager SyncManager = new SyncManager(LookupTable);
        private static UpdateManager UpdataManager = new UpdateManager(LookupTable);

        internal static void AutoSync()
        {
            ConsoleLog.Title("Auto sync changes made in netfx projects to netcore projects:");
            ConsoleLog.Ignore("----------------------------------------------------------------");

            using (Timer.Time)
            {
                ConsoleLog.Title("Scaning projects ...............");
                SyncManager.ScanProducedProjects();

                ConsoleLog.Title("Caching projects on master .....");
                SyncManager.CacheMasterProjects();

                ConsoleLog.Title("Caching projects on dogfood ....");
                SyncManager.CacheDogFoodProjects();

                ConsoleLog.Title("Syncing changes ................");
                SyncManager.CompareAndApply();
            }
        }

        internal static void UpdatePackagesProps()
        {
            using (Timer.Time)
            {
                ConsoleLog.Title("Upgrade Substrate packages .....");
                UpdataManager.UpdateSubstratePackages();

                ConsoleLog.Title("Update hardcodedbldver.h .....");
                UpdataManager.UpdateHardCodedbldver();
            }
        }

        internal static void UpdateNuGetPackages()
        {
            using (Timer.Time)
            {
                ConsoleLog.Title("Upgrade NuGet packages .....");
                UpdataManager.UpdateNuGetPackages();
            }
        }

        internal static void ConsumeCso()
        {
            throw new NotImplementedException();
        }

        internal static void ConsumePop3()
        {
            throw new NotImplementedException();
        }

        internal static void OpenSettings()
        {
            throw new NotImplementedException();
        }

        internal static void ShowCommands()
        {
            // manually, just in case
            // ConsoleLog.Message("\nRun this command in enlistment to start manually merge:");
            // ConsoleLog.Warning(Commands.DiffAll());
            ConsoleLog.Message("\nRun this command in enlistment to start merge check:");
            string commands = string.Format(@"for /f %i in ({0}) do git difftool {1}...{2} -- %i",
                                            AppSettings.ChangedFiles,
                                            AppSettings.DFCommitID,
                                            AppSettings.MainCommitID);
            ConsoleLog.Warning(commands);
        }
    }
}

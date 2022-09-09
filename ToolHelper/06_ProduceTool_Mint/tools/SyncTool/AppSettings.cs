namespace SyncTool
{
    using System.IO;
    using Mint.Common.Utilities;

    static class AppSettings
    {
        private static readonly string AppRoot = PathUtils.ApplicationRoot();
        private static readonly SettingFile Settings = new SettingFile(Path.Combine(AppRoot, @"merge.settings.ini"));

        public static readonly string MainBranch     = Settings.Read("Main",    "Branch");
        public static readonly string DFBranch       = Settings.Read("DF",      "Branch");
        public static readonly string FixBranch      = Settings.Read("Fix",     "Branch");
        public static readonly string MainCommitID   = Settings.Read("Main",    "CommitID");
        public static readonly string DFCommitID     = Settings.Read("DF",      "CommitID");
        public static readonly string DFSrc          = Settings.Read("DF",      "LocalSrc");
        public static readonly string CSOSrc         = Settings.Read("Cso",     "LocalSrc");
        public static readonly string Pop3Src        = Settings.Read("Pop3",    "LocalSrc");
        public static readonly string BuildVersion   = Settings.Read("Build",   "Version");
        public static readonly string PackageVersion = Settings.Read("Package", "Version");

        public static readonly string ChangedFiles   = Path.Combine(AppRoot, @"_ChangedFiles.txt");
    }
}

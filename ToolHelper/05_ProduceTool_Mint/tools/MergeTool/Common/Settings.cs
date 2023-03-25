namespace MergeTool.Common
{
    using Mint.Common;

    static class Settings
    {
        private static readonly Setting _manager = new Setting(@"merge.settings.ini");

        public static readonly string FilePath = _manager.FilePath;

        public static readonly string MainBranch = _manager.Read("Main", "Branch");
        public static readonly string DFBranch = _manager.Read("DF", "Branch");
        public static readonly string FixBranch = _manager.Read("Fix", "Branch");

        public static readonly string MainCommitID = _manager.Read("Main", "CommitID");
        public static readonly string DFCommitID = _manager.Read("DF", "CommitID");

        public static readonly string DFSrc = _manager.Read("DF", "LocalSrc");
        public static readonly string CSOSrc = _manager.Read("Cso", "LocalSrc");
        public static readonly string Pop3Src = _manager.Read("Pop3", "LocalSrc");

        public static readonly string BuildVersion = _manager.Read("Build", "Version");
        public static readonly string PackageVersion = _manager.Read("Package", "Version");
    }
}

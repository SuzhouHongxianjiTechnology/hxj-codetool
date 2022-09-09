#nullable disable

namespace Mint.Substrate.Configurations
{
    using System.IO;
    using Mint.Common.Utilities;

    public class SubstratePaths
    {
        public string SrcDir { get; set; }

        public string RestoreEntry { get; set; }

        public string MasterRestoreEntry { get; set; }

        public string PackagesProps { get; set; }

        public string InnerCorext { get; set; }

        public string OuterCorext { get; set; }

        public string VariantConfigV1 { get; set; }

        public string VariantConfigV2 { get; set; }

        public string HardCodedBuildVersion { get; set; }

        // ------------------------------------------------------------

        public static SubstratePaths Settings = SubstratePaths.Load();

        private static SubstratePaths Load()
        {
            string defaultSource = Path.Combine(PathUtils.ApplicationFolder("settings"), "substrate.paths.json");
            return FileUtils.DeserializeJson<SubstratePaths>(defaultSource);
        }
    }
}

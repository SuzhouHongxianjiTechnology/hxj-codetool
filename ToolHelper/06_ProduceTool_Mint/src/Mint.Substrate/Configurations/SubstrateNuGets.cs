#nullable disable

namespace Mint.Substrate.Configurations
{
    using System.Collections.Generic;
    using System.IO;
    using Mint.Common.Utilities;

    public class SubstrateNuGets
    {
        public HashSet<string> KnownSDKs { get; set; }

        public HashSet<string> KnownNuGets { get; set; }

        public HashSet<string> ExcludedNuGets { get; set; }

        public Dictionary<string, string> NuGetReplacement { get; set; }

        // ------------------------------------------------------------

        public static SubstrateNuGets Settings = SubstrateNuGets.Load();

        private static SubstrateNuGets Load()
        {
            string defaultSource = Path.Combine(PathUtils.ApplicationFolder("settings"), "substrate.nugets.json");
            return FileUtils.DeserializeJson<SubstrateNuGets>(defaultSource);
        }
    }
}

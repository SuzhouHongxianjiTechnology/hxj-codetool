#nullable disable

namespace Mint.Substrate.Configurations
{
    using System.Collections.Generic;
    using System.IO;
    using Mint.Common.Utilities;

    public class SubstrateProperties
    {
        public Dictionary<string, string> IncludeProperties { get; set; }

        public HashSet<string> ExcludedProperties { get; set; }

        public HashSet<string> KnownProperties { get; set; }

        // ------------------------------------------------------------

        public static SubstrateProperties Settings = SubstrateProperties.Load();

        private static SubstrateProperties Load()
        {
            string defaultSource = Path.Combine(PathUtils.ApplicationFolder("settings"), "substrate.properties.json");
            return FileUtils.DeserializeJson<SubstrateProperties>(defaultSource);
        }
    }
}

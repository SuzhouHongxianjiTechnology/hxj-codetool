#nullable disable

namespace Mint.Substrate.Configurations
{
    using System.Collections.Generic;
    using System.IO;
    using Mint.Common.Utilities;

    public class SubstrateProjects
    {
        public HashSet<string> CppProjects { get; set; }

        public HashSet<string> SubstrateNuspecs { get; set; }

        // ------------------------------------------------------------

        public static SubstrateProjects Settings = SubstrateProjects.Load();

        private static SubstrateProjects Load()
        {
            string defaultSource = Path.Combine(PathUtils.ApplicationFolder("settings"), "substrate.projects.json");
            return FileUtils.DeserializeJson<SubstrateProjects>(defaultSource);
        }
    }
}

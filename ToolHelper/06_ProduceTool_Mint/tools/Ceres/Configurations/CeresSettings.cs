#nullable disable

namespace Ceres
{
    using System.IO;
    using Mint.Common.Utilities;

    public class CeresSettings
    {
        public string FastRepoSrcDir { get; set; }

        public static CeresSettings Settings = CeresSettings.Load();

        private static CeresSettings Load()
        {
            string defaultSource = Path.Combine(PathUtils.ApplicationFolder("settings"), "ceres.settings.json");
            return FileUtils.DeserializeJson<CeresSettings>(defaultSource);
        }
    }
}

#nullable disable

namespace Mint.Database.Configurations
{
    using System.IO;
    using Mint.Common.Utilities;

    public class DatabaseSettings
    {
        public string AllTypesJson { get; set; }

        public string ProductionURL { get; set; }

        public string Neo4j_Host { get; set; }

        public string Neo4j_User { get; set; }

        public string Neo4j_Pass { get; set; }

        // ------------------------------------------------------------

        public static DatabaseSettings Settings = DatabaseSettings.Load();

        private static DatabaseSettings Load()
        {
            string defaultSource = Path.Combine(PathUtils.ApplicationFolder("settings"), "database.settings.json");
            return FileUtils.DeserializeJson<DatabaseSettings>(defaultSource);
        }
    }
}

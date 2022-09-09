namespace Mint.Database.Remote
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class Versions
    {
        private const string URL_ALL_VERSIONS = @"all-versions";

        private const string URL_LATEST_VERSION = @"latest-version";

        public static async Task<List<string>> GetAllVersionsAsync()
        {
            return await HttpRequest.GetAsync<List<string>>(Versions.URL_ALL_VERSIONS);
        }

        public static async Task<string> GetLatestVersionAsync()
        {
            return await HttpRequest.GetAsync(Versions.URL_LATEST_VERSION);
        }
    }
}

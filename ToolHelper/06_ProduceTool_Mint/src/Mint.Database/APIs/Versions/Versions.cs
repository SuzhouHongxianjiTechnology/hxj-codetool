namespace Mint.Database.APIs
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Mint.Database.Connection;

    public class Versions
    {
        private const string URL_ALL_VERSIONS = @"all-versions";

        private const string URL_LATEST_VERSION = @"latest-version";

        public static async Task<List<string>> AllVersionsAsync()
        {
            return await HttpRequest.GetAsync<List<string>>(Versions.URL_ALL_VERSIONS);
        }

        public static async Task<string> LatestVersionAsync()
        {
            return await HttpRequest.GetStringAsync(Versions.URL_LATEST_VERSION);
        }
    }
}

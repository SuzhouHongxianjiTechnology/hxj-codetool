
namespace Mint.Database.Remote
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class ProcessAssemblies
    {
        private const string URL = @"process/assemblies" +
                                   @"?process={0}" +
                                   @"&version={1}" +
                                   @"&isCompatible=" +
                                   @"&source={2}" +
                                   @"&isPackageCompatible=" +
                                   @"&projectType=" +
                                   @"&dereferenceType=0" +
                                   @"&isFiltered=" +
                                   @"&assemblyFilter=" +
                                   @"&page=1" +
                                   @"&size=2147483647";

        public static async Task<HashSet<string>> RequestAsync(string version, int source, Process process = Process.None)
        {
            string processName = process == Process.None ? string.Empty : process.ToString();
            string url = string.Format(URL, processName, version, source);
            var response = await HttpRequest.GetStirngAsync(url);
            var document = JsonDocument.Parse(response);
            var assemblyNames = document.RootElement
                                        .GetProperty("data")
                                        .EnumerateArray()
                                        .Select(a => a.GetProperty("assemblyName").GetString())
                                        .ToHashSet();
            return assemblyNames;
        }
    }
}

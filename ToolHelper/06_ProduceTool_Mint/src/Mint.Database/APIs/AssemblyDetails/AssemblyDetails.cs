namespace Mint.Database.APIs
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Mint.Database.Connection;
    using Mint.Database.Enums;

    public class AssemblyDetails
    {
        private const string URL = @"assembly/details" +
                                   @"?assembly={0}" +
                                   @"&version={1}" +
                                   @"&process={2}";

        public static async Task<AssemblyDetails> RequestAsync(string assembly, string version = null, Process process = Process.None)
        {
            assembly = assembly.EndsWith(".dll") || assembly.EndsWith(".exe") ? assembly : assembly + ".dll";
            version = version ?? await Versions.LatestVersionAsync();
            string processName = process == Process.None ? string.Empty : process.ToString();
            string url = string.Format(URL, assembly, version, processName);
            return await HttpRequest.GetAsync<AssemblyDetails>(url);
        }

        // ------------------------------------------------------------

        public string AssemblyName { get; set; }
        public string Version { get; set; }
        public string SourcePath { get; set; }
        public string Location { get; set; }
        public AssemblySource Source => Type;
        public List<string> ReferencedIncompatibleAPIs { get; set; }
        public List<string> FilteredAPIs { get; set; }
        public List<string> Processes { get; set; }
        public Compatibility CompatibleResult { get; set; }
        public AssemblySource Type { get; set; }
        public string PackageName { get; set; }
        public string PackageVersion { get; set; }
        public List<string> TargetFrameworks { get; set; }
        public int Distance { get; set; }
        public bool HasReflectionCall { get; set; }
        public bool ReflectionNeedManualCheck { get; set; }
        public int ChildCount { get; set; }
        public int ParentCount { get; set; }
        public int DescendantCount { get; set; }
        public string Feed { get; set; }
        public string FeedLocation { get; set; }
        public bool HasCycle { get; set; }
        public bool? Dereferenced { get; set; }
        public Properties Properties { get; set; }
    }

    public enum AssemblySource
    {
        All,
        Nuget,
        Substrate
    }

    public enum Compatibility
    {
        InCompatible,
        Compatible,
        FilterCompatible
    }

    public class Properties
    {
        public NugetInfo NugetInfo { get; set; }
        public ProducedInfo ProducedInfo { get; set; }
    }

    public class NugetInfo
    {
        public string HasNetCoreVersion { get; set; }

        public List<string> PackageInfo { get; set; }
    }

    public class ProducedInfo
    {
        public string producedState { get; set; }

        public string ProducedAuthor { get; set; }

        public string ProducedComment { get; set; }
    }

}

namespace MapiAnalyser.Cache
{
    using System.Collections.Generic;

    public class AssemblyData
    {
        public string FilePath { get; set; }

        public string AssemblyName { get; set; }

        public bool IsProduced { get; set; }

        public List<string> BlockedBy { get; set; }

        public List<string> IncompatibleAPIs { get; set; }

        public List<string> FilteredAPIs { get; set; }
    }
}

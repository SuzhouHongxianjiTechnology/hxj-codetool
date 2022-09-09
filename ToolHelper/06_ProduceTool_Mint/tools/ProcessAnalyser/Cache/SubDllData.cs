#nullable disable

namespace ProcessAnalyser
{
    using System.Collections.Generic;

    public class SubDllData
    {
        public string FilePath { get; set; }

        public string AssemblyName { get; set; }

        public bool IsProduced { get; set; }

        public List<string> BlockedSub { get; set; }

        public List<string> BlockedNonSub { get; set; }

        public List<string> IncompatibleAPIs { get; set; }

        public List<string> FilteredAPIs { get; set; }

        public object GetProperty(string propertyName)
        {
            return this.GetType().GetProperty(propertyName).GetValue(this, null);
        }
    }
}

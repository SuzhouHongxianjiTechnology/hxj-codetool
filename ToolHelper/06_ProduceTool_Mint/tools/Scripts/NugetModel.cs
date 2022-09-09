using System.Collections.Generic;

namespace Scripts
{
    public class NugetModel
    {
        public NugetModel(string name, List<DllModel> dlls)
        {
            PackageName = name;
            IncludedDLLs = dlls;
        }

        public string PackageName { get; set; }

        public List<DllModel> IncludedDLLs { get; set; }

    }

    public class DllModel
    {
        public DllModel(string assemblyName, List<string> incompatibleAPIs)
        {
            AssemblyName = assemblyName;
            IncompatibleAPIs = incompatibleAPIs;
        }

        public string AssemblyName { get; set; }

        public List<string> IncompatibleAPIs { get; set; }
    }
}

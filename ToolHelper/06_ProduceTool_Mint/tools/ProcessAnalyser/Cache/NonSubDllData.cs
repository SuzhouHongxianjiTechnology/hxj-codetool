#nullable disable

namespace ProcessAnalyser
{
    using System.Collections.Generic;

    public class NonSubDllData
    {
        public string DllName { get; set; }

        public string PackageName { get; set; }

        public bool IsCompatible { get; set; }

        public List<string> Replacenment { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Albert.Model
{
    public class Error
    {
        public Error() { }
        public Error(string errorcode, string projectPath, string packageName, string version)
        {
            this.ErrorCode = errorcode;
            this.PackageName = packageName;
            this.Version = version;
            this.ProjectPath = projectPath;
        }

        public string ErrorCode { get; set; }
        public string PackageName { get; set; }
        public string Version { get; set; }
        public string ProjectPath { get; set; }
    }
}

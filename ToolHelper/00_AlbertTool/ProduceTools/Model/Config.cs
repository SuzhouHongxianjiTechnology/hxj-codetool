using System;
using System.Collections.Generic;
using System.Text;

namespace Albert.Model
{
    class Config
    {
        public string RepoLink { get; set; }
        public string GitRootDir { get; set; }
        public Addressee Addressees { get; set; }
        public List<ProjectPath> BuildList { get; set; }
        public Dictionary<string, string> RequiredPolicies { get; set; }
    }
}

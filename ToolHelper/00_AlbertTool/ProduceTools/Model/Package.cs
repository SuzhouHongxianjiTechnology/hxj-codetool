using System;
using System.Collections.Generic;
using System.Text;

namespace Albert.Model
{
    class Package
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public List<PackageVersion> Versions { get; set; }
    }
}

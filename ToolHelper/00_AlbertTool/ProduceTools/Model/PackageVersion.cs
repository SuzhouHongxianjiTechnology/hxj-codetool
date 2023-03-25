using System;
using System.Collections.Generic;
using System.Text;

namespace Albert.Model
{
    class PackageVersion
    {
        public string Id { get; set; }

        public string normalizedVersion { get; set; }

        public string publishDate { get; set; }
    }
}

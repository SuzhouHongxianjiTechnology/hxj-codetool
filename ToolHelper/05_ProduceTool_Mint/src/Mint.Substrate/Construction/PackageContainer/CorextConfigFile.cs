namespace Mint.Substrate.Construction
{
    using System;
    using System.Collections.Generic;
    using Mint.Common;

    public sealed class CorextConfigFile : PackageContainer
    {
        internal protected override bool ReadOnly => true;

        public CorextConfigFile(string path) : base(path) { }

        protected override void CachePackages()
        {
            this.Packages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var package in this.Document.GetFirst(Tags.Packages).GetAll(Tags.Package))
            {
                string name = package.GetAttribute(Tags.Id).Value;
                string version = package.GetAttribute(Tags.Version).Value;
                if (this.Packages.ContainsKey(name))
                {
                    // corext.config could have same packages with different versions,
                    // normally those packages are orderd by versions,
                    // it should be safe to use the last one.
                    this.Packages[name] = version;
                }
                else
                {
                    this.Packages.Add(name, version);
                }
            }
        }
    }
}

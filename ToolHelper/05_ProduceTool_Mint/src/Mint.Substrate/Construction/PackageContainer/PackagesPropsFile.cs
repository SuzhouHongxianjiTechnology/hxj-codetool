namespace Mint.Substrate.Construction
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Mint.Common;

    public sealed class PackagesPropsFile : PackageContainer
    {
        internal protected override bool ReadOnly => false;

        public PackagesPropsFile(string path) : base(path) { }

        protected override void CachePackages()
        {
            this.Packages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var package in this.Document.GetAll(Tags.PackageReference))
            {
                string name = package.GetAttribute(Tags.Update).Value;
                string version = package.GetAttribute(Tags.Version).Value;
                this.Packages.TryAdd(name, version);
            }
        }

        public string GetPackageName(string packageName)
        {
            return this.Packages.Keys.Where(k => StringUtils.EqualsIgnoreCase(k, packageName)).FirstOrDefault();
        }

        public void SetPackageVersion(string name, string version)
        {
            if (!this.HasPackage(name)) return;

            this.Packages[name] = version;
            var element = this.Document.GetAll(Tags.PackageReference)
                                       .Where(p => StringUtils.EqualsIgnoreCase(name, p.GetAttribute(Tags.Update).Value))
                                       .First();
            element.SetAttributeValue(Tags.Version, version);
        }

        public void UpgradePackageVersion(string name, string version)
        {
            if (!this.HasPackage(name)) return;

            string thisVersion = this.Packages[name];
            if (StringUtils.EqualsIgnoreCase(thisVersion, version)) return;

            try
            {
                Version t = new Version(thisVersion);
                Version o = new Version(version);
                if (t.CompareTo(o) < 0)
                {
                    this.SetPackageVersion(name, version);
                }
            }
            catch (FormatException)
            {
                throw new Exception(
                    string.Format("[{0}] {1}  >>  {2}", name, thisVersion, version)
                );
            }
        }
    }
}

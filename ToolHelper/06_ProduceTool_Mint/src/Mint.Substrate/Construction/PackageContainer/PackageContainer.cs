namespace Mint.Substrate.Construction
{
    using System.Collections.Generic;

    public abstract class PackageContainer : XmlFile
    {
        protected abstract void CachePackages();

        public Dictionary<string, string> Packages { get; protected set; }

        public PackageContainer(string path) : base(path)
        {
            this.CachePackages();
        }

        public bool HasPackage(string packageName)
        {
            return this.Packages.ContainsKey(packageName);
        }

        public string GetPackageVersion(string packageName)
        {
            this.Packages.TryGetValue(packageName, out string version);
            return version;
        }
    }
}

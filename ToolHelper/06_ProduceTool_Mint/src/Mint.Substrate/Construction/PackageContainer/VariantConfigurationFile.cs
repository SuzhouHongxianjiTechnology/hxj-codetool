namespace Mint.Substrate.Construction
{
    using System;
    using System.Collections.Generic;
    using Mint.Common;

    public sealed class VariantConfigurationFile : PackageContainer
    {
        internal protected override bool ReadOnly => true;

        public VariantConfigurationFile(string path) : base(path) { }

        protected override void CachePackages()
        {
            this.Packages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var package in this.Document.GetAll(Tags.PackageReference))
            {
                string name = package.GetAttribute(Tags.Include).Value;
                this.Packages.TryAdd(name, null);
            }
        }
    }
}

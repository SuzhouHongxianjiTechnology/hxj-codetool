namespace Mint.Substrate.Construction
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Xml.Linq;
    using Mint.Common.Collections;

    public class PackageSet
    {
        private IgnoreCaseDictionary<Package> packages = new IgnoreCaseDictionary<Package>();

        public PackageSet(IEnumerable<XElement> pkgElements, string nameTag, string versionTag)
        {
            foreach (var pkgElement in pkgElements)
            {
                var package = new Package(pkgElement, nameTag, versionTag);
                this.packages.TryAdd(package.Name, package);
            }
        }

        public Package this[string name] => this.packages[name];

        public Dictionary<string, string> AsDictionary()
        {
            return this.packages.Values.ToDictionary(p => p.Name, p => p.Version);
        }

        public bool TryGetName(string name, [MaybeNullWhen(false)] out string actualName)
        {
            return this.packages.TryGetKey(name, out actualName);
        }

        public bool TryGetPackage(string name, [MaybeNullWhen(false)] out Package package)
        {
            return this.packages.TryGetValue(name, out package);
        }

        public bool Has(string name)
        {
            return this.packages.ContainsKey(name);
        }

        public bool TryUpgrade(string name, string version)
        {
            return Has(name) && this.packages[name].TryUpgrade(version);
        }

        public void Upgrade(PackageSet other, out List<(string Name, string CurrentVersion, string NewVersion)> fails)
        {
            fails = new List<(string Name, string CurrentVersion, string NewVersion)>();

            foreach (var package in other.packages.Values)
            {
                string name = package.Name;
                if (!Has(name)) continue;

                string version = package.Version;
                if (!TryUpgrade(name, version))
                {
                    fails.Add((
                        Name: name,
                        CurrentVersion: this.packages[name].Version,
                        NewVersion: version
                    ));
                }
            }
        }
    }
}

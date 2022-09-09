namespace Mint.Substrate.Construction
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Xml.Linq;
    using Mint.Common;

    public class NuspecProjectFile : PackageContainer
    {
        internal protected override bool ReadOnly => false;

        private const string _NETSTD_LIB = @"lib\netstandard2.0";

        private const string _NETCORE_LIB = @"lib\netcoreapp3.1";

        public string Id => this.Document.GetFirst(Tags.Id).Value;

        public string Version
        {
            get => this.Document.GetFirst(Tags.Version).Value;
            set => this.Document.GetFirst(Tags.Version).SetValue(value);
        }

        public string Description
        {
            get => this.Document.GetFirst(Tags.Description).Value;
            set => this.Document.GetFirst(Tags.Description).SetValue(value);
        }

        public List<string> IncludedFiles { get; }

        public NuspecProjectFile(string path) : base(path)
        {
            this.IncludedFiles = this.Document.GetAll(Tags.File)
                                              .Where(file => file.HasAttribute(Tags.Target, _NETCORE_LIB) || file.HasAttribute(Tags.Target, _NETSTD_LIB))
                                              .Select(file => file.GetAttribute(Tags.Src).Value)
                                              .ToList();
        }

        protected override void CachePackages()
        {
            this.Packages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var package in this.Document.GetAll(Tags.Dependency))
            {
                string name = package.GetAttribute(Tags.Id).Value;
                string version = package.GetAttribute(Tags.Version).Value;
                this.Packages.TryAdd(name, version);
            }
        }

        public void SetDependencyVersion(string name, string version)
        {
            if (!this.HasPackage(name)) return;

            this.Packages[name] = version;
            var element = this.Document.GetAll(Tags.Dependency)
                                       .Where(p => StringUtils.EqualsIgnoreCase(name, p.GetAttribute(Tags.Id).Value))
                                       .First();
            element.SetAttributeValue("version", version);
        }

        public void ReplaceDependencies(OrderedDictionary dependencies)
        {
            // For now, we onlt replace: <group targetFramework=".NETCoreApp3.1">
            var dependencyGroup = this.Document.GetFirst(Tags.Group);
            dependencyGroup.RemoveNodes();

            foreach (var name in dependencies.Keys)
            {
                var element = new XElement(this.Namespace + "dependency",
                                           new XAttribute("id", name),
                                           new XAttribute("version", dependencies[name]));
                dependencyGroup.Add(element);
            }

            this.CachePackages();
        }
    }
}

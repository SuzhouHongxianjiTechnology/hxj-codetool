namespace Mint.Substrate.Construction
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using Mint.Common;
    using Mint.Substrate.Constants;
    using Mint.Substrate.Utilities;

    public class RestoreEntryFile : PathContainer
    {
        internal protected override bool ReadOnly => false;

        public RestoreEntryFile(string path) : base(path)
        {
            CacheProjectPaths();
        }

        public override void AddPath(string path)
        {
            var element = new XElement(this.Namespace + Tags.ProjectFile, new XAttribute(Tags.Include, path));
            this.Document.GetFirst(Tags.ItemGroup).AddFirst(element);
        }

        public override void RemovePath(string path)
        {
            foreach (var file in this.Document.GetAll(Tags.ProjectFile))
            {
                string filePath = file.GetAttribute(Tags.Include).Value;
                if (StringUtils.EqualsIgnoreCase(path, filePath))
                {
                    file.TryRemove();
                }
            }
        }

        public List<string> ProjectPaths => this.Document.GetAll(Tags.ProjectFile)
                                                         .Select(p => p.GetAttribute(Tags.Include).Value)
                                                         .Select(v => StringUtils.ReplaceIgnoreCase(v, "$(InetRoot)", DF.SrcDir))
                                                         .ToList();

        public Dictionary<string, (string, string)> Cache { get; private set; }

        private void CacheProjectPaths()
        {
            this.Cache = new Dictionary<string, (string, string)>(StringComparer.OrdinalIgnoreCase);

            foreach (var projectPath in this.ProjectPaths)
            {
                if (!File.Exists(projectPath))
                {
                    continue;
                }
                var projectFile = new NetCoreProjectFile(projectPath);
                string assembly = projectFile.AssemblyName;
                string targetPath = projectFile.TargetPath;

                if (!string.IsNullOrEmpty(assembly))
                {
                    this.Cache.TryAdd(assembly, (projectPath, targetPath));
                }
            }

            foreach (var project in SpecialCases.AdditinalProjects)
            {
                this.Cache.TryAdd(project.Key, project.Value);
            }
        }

        public bool TryGetProjectPathsByName(string assembly, out string projectPath, out string targetPath)
        {
            assembly = assembly.EndsWith(".dll") ? assembly.Substring(0, assembly.Length - 4) : assembly;
            projectPath = null;
            targetPath = null;
            if (this.Cache.ContainsKey(assembly))
            {
                projectPath = this.Cache[assembly].Item1;
                targetPath = this.Cache[assembly].Item2;
                return true;
            }
            return false;
        }

        public void SortAndRemoveDuplicates()
        {
            this.Document.GetFirst(Tags.ItemGroup).SortByAttribute(Tags.Include);
            var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in this.Document.GetAll(Tags.ProjectFile))
            {
                string include = item.GetAttribute(Tags.Include)?.Value;
                if (!paths.Contains(include))
                {
                    paths.Add(include);
                }
                else
                {
                    item.TryRemove();
                }
            }
            CacheProjectPaths();
        }

        public bool IsProduced(string assembly)
        {
            assembly = assembly.EndsWith(".dll") ? assembly.Substring(0, assembly.Length - 4) : assembly;
            return this.Cache.ContainsKey(assembly);
        }
    }
}

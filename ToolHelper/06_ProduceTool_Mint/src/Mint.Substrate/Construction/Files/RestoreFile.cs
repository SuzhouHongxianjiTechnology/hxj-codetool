namespace Mint.Substrate.Construction
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Mint.Common.Extensions;

    public class RestoreFile : XMLFile
    {
        private HashSet<string> Paths => this.Document.GetAll(Tags.ProjectFile)
                                             .Select(p => p.GetAttribute(Tags.Include).Value.Trim()) // FORMAT! FFS!
                                             .ToHashSet(StringComparer.OrdinalIgnoreCase);

        public RestoreFile(string path) : base(path)
        {
        }

        public ProjectSet GetProjects(string srcDir, ProjectResolver resolver)
        {
            var localPaths = this.Paths.Select(p => p.ReplaceIgnoreCase("$(InetRoot)", srcDir));
            return new ProjectSet(localPaths, resolver);
        }

        public void AddPath(string inetPath)
        {
            if (this.Paths.Contains(inetPath))
            {
                return;
            }

            var group = this.Document.GetFirst(Tags.ItemGroup);
            group.AddFirst(
                new XElement(
                    Namespace + Tags.ProjectFile,
                    new XAttribute(Tags.Include, inetPath)
                )
            );
        }

        public void RemovePath(string inetPath)
        {
            foreach (var item in this.Document.GetAll(Tags.ProjectFile).ToList())
            {
                if (item.HasAttribute(Tags.Include, inetPath))
                {
                    item.TryRemove();
                }
            }
        }

        public void ReplacePath(string oldPath, string newPath)
        {
            foreach (var item in this.Document.GetAll(Tags.ProjectFile).ToList())
            {
                if (item.HasAttribute(Tags.Include, oldPath))
                {
                    item.SetAttributeValue(Tags.Include, newPath);
                }
            }
        }

        public void OrganizeProjects()
        {
            HashSet<string> pathSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in this.Document.GetAll(Tags.ProjectFile).ToList())
            {
                string path = item.GetAttribute(Tags.Include).Value;
                if (pathSet.Contains(path))
                {
                    item.TryRemove();
                }
                pathSet.Add(path);
            }

            foreach (var group in this.Document.GetAll(Tags.ItemGroup))
            {
                group.SortByAttribute(Tags.Include);
            }
        }
    }
}

namespace Mint.Substrate.Construction
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public class ProjectSet : IEnumerable<IProject>
    {
        private Dictionary<string, IProject> projects = new Dictionary<string, IProject>(StringComparer.OrdinalIgnoreCase);

        public ProjectSet(IEnumerable<string> paths, ProjectResolver resolver)
        {
            foreach (var path in paths)
            {
                var project = resolver.Resolve(path);
                this.projects.TryAdd(project.Name, project);
            }
        }

        public ProjectSet(IEnumerable<IProject> projects)
        {
            this.projects = new Dictionary<string, IProject>(StringComparer.OrdinalIgnoreCase);
            foreach (var project in projects)
            {
                this.projects.TryAdd(project.Name, project);
            }
        }

        public IProject this[string name] => this.projects[name];

        public Dictionary<string, IProject> AsDictionary()
        {
            return this.projects;
        }

        public bool Has(string name)
        {
            return this.projects.ContainsKey(name);
        }

        public bool TryGetProject(string name, [MaybeNullWhen(false)] out IProject project)
        {
            return this.projects.TryGetValue(name, out project);
        }

        public string? GetFilePath(string name)
        {
            return this.projects.ContainsKey(name) ? this.projects[name].FilePath : null;
        }

        public IEnumerator<IProject> GetEnumerator()
        {
            foreach (var project in this.projects.Values)
            {
                yield return project;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

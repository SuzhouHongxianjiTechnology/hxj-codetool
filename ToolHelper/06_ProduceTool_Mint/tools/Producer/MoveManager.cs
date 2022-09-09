namespace Producer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using Mint.Common.Extensions;
    using Mint.Common.Utilities;
    using Mint.Substrate;
    using Mint.Substrate.Construction;
    using Mint.Substrate.Utilities;

    internal class MoveManager
    {
        private static readonly HashSet<string> IgnoreComponents = new HashSet<string>
        {
            "NetFxToNetStdNetCore"
        };

        internal static void MoveComponents(string component)
        {
            var components = GetComponents();
            ConsoleLog.Warning($"Total components left: {components.Count}");
            ConsoleLog.Ignore("----------------------------------------------------------------");

            if (component.Equals("left"))
            {
                int total = 0;
                foreach (var c in components.Keys)
                {
                    total += components[c].Count();
                    ConsoleLog.Ignore($"{c} - {components[c].Count()}");
                }
                ConsoleLog.Highlight($"Total: {total}");
                return;
            }

            if (components.ContainsKey(component))
            {
                string name = component;
                MoveComponent(name, components[name]);
            }
            else
            {
                ConsoleLog.Error($"'{component}' is not a valid component or it has been moved already");
            }
        }

        private static void MoveComponent(string component, List<string> projects)
        {
            var projResolver = new ProjectResolver();
            ConsoleLog.Highlight($" - Component: {component} - Projects: {projects.Count}");
            string newComponent = component + ".NetCore";
            string devFolder = Path.Combine(Repo.Paths.SrcDir, "sources", "dev");

            // setup root dirs file
            string rootDirs = Path.Combine(devFolder, "dirs");
            var rootDirsFile = Repo.LoadTextDirsFile(rootDirs);
            rootDirsFile.AddPathAfter(component, newComponent);
            rootDirsFile.Save();

            //
            var moved = new Dictionary<string, IProject>(StringComparer.OrdinalIgnoreCase);

            // move projects
            var restoreEntry = Repo.RestoreEntry;
            foreach (var project in projects)
            {
                string newFile = project.ReplaceIgnoreCase($@"sources\dev\{component}", $@"sources\dev\{newComponent}");
                MoveProject(project, newFile, component, newComponent);

                if (!project.ContainsIgnoreCase("NUPKG"))
                {
                    FormatNewProject(project, newFile);
                }

                // replace project path in restore entry
                var oldPath = project.Replace(Repo.Paths.SrcDir, "$(Inetroot)");
                var newPath = newFile.Replace(Repo.Paths.SrcDir, "$(Inetroot)");
                restoreEntry.ReplacePath(oldPath, newPath);

                var projectItem = projResolver.Resolve(newPath);
                moved.Add(projectItem.Name, projectItem);
            }
            restoreEntry.Document.GetFirst(Tags.ItemGroup).SortByAttribute(Tags.Include);
            restoreEntry.Save();

            // create dirs file
            CreateDirsFile(Path.Combine(devFolder, newComponent), isRoot: true);
        }

        internal static void ReplaceTargetPath()
        {
            var lookup = new LookupTable();

            var resolver = new ProjectResolver();
            var paths = Repo.RestoreEntry.GetProjects(Repo.Paths.SrcDir, resolver).Select(p => p.FilePath).ToList();

            foreach (var path in paths)
            {
                if (path.Contains("sources\\test"))
                {
                    continue;
                }

                var file = Repo.Load<BuildFile>(path);
                bool changed = false;
                foreach (var reference in file.Document.GetAll(Tags.Reference))
                {
                    string name = reference.GetAttribute(Tags.Include).Value;

                    if (lookup.IsProducedProject(name, out IProject? produced))
                    {
                        var pathItem = reference.GetFirst(Tags.HintPath);

                        var curPath = pathItem.Value;
                        var expPath = produced.TargetPath;

                        if (!curPath.Replace("\\", "").EqualsIgnoreCase(expPath?.Replace("\\", "")))
                        {
                            pathItem.SetValue(expPath);
                            changed = true;
                        }
                    }
                }

                if (changed) file.Save(omniDeclaration: true);
            }
        }

        private static Dictionary<string, List<string>> GetComponents()
        {
            var components = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            var projects = Repo.RestoreEntry.Document.GetAll(Tags.ProjectFile);
            foreach (var project in projects)
            {
                string inetRoot = project.GetAttribute(Tags.Include).Value;
                if (inetRoot.ContainsIgnoreCase("nupkg"))
                {
                    continue;
                }

                if (!inetRoot.StartsWithIgnoreCase(@"$(Inetroot)\sources\dev\"))
                {
                    continue;
                }

                string componentName = inetRoot.ReplaceIgnoreCase(@"$(Inetroot)\sources\dev\", "").Split('\\').First();

                if (IgnoreComponents.Contains(componentName, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (componentName.EndsWithIgnoreCase(".NetCore") || componentName.EndsWithIgnoreCase(".NetStd"))
                {
                    continue;
                }

                if (!components.ContainsKey(componentName))
                {
                    components[componentName] = new List<string>();
                }

                string fullPath = inetRoot.ReplaceIgnoreCase("$(Inetroot)", Repo.Paths.SrcDir);
                components[componentName].Add(fullPath);
            }
            return components;
        }

        private static void MoveProject(string oldFile, string newFile, string component, string newComponent)
        {
            string? oldFolder = Path.GetDirectoryName(oldFile);
            string? newFolder = Path.GetDirectoryName(newFile);
            if (string.IsNullOrEmpty(oldFolder) || string.IsNullOrEmpty(newFolder))
            {
                throw new Exception("should never happend.");
            }

            // create new file
            if (!Directory.Exists(newFolder))
            {
                Directory.CreateDirectory(newFolder);
            }

            foreach (string item in Directory.GetFiles(oldFolder, "*.*", SearchOption.AllDirectories))
            {
                FileUtils.CopyFile(item, item.Replace(oldFolder, newFolder));
            }

            // cleanup old files
            Directory.Delete(oldFolder, recursive: true);

            // cleanup old dirs
            string oldDirs = Path.Combine(Directory.GetParent(oldFolder).ToString(), "dirs");
            string oldDirsItemName = oldFolder.Split('\\').Last();
            var oldDirsFile = Repo.LoadTextDirsFile(oldDirs);
            oldDirsFile.RemovePath(oldDirsItemName);
            oldDirsFile.Save();
        }

        private static void FormatNewProject(string oldFile, string fullPath)
        {
            var file = Repo.Load<BuildFile>(fullPath);

            try
            {
                var mainGroup = file.Document.GetAll(Tags.PropertyGroup)
                                             .Where(group => group.GetFirst(Tags.AssemblyName) != null)
                                             .First();
                if (mainGroup == null)
                {
                    mainGroup = file.Document.GetFirst(Tags.PropertyGroup);
                }

                // add ProjectPath property
                string fxFile = MSBuildUtils.InferNFBuildFileByPath(oldFile);
                string oldParent = Directory.GetParent(fxFile).ToString().ReplaceIgnoreCase(Repo.Paths.SrcDir, "$(Inetroot)");
                string oldParentFolder = oldParent.Split('\\').Last();
                var projectPath = file.Document.GetFirst(Tags.ProjectPath);
                if (projectPath == null)
                {
                    var prop = new XElement(Tags.ProjectPath);
                    prop.SetValue(oldParent);
                    mainGroup.Add(prop);
                }
                else
                {
                    projectPath.SetValue(oldParent);
                }

                // replace any include with new ProjectPath property
                foreach (var element in file.Document.Descendants().ToList())
                {
                    if (element.HasAttribute(Tags.Include))
                    {
                        var include = element.GetAttribute(Tags.Include);
                        string value = include.Value;
                        string relPath = $"..\\{oldParentFolder}";

                        // things gets ugly here
                        if (value.StartsWith("$"))
                        {
                            MSBuildUtils.TryResolveBuildVariables(oldParent, value, out string normalPath);
                            if (normalPath.StartsWithIgnoreCase(relPath))
                            {
                                var newValue = normalPath.ReplaceIgnoreCase(relPath, "$(ProjectPath)");
                                include.SetValue(newValue);
                            }
                            else
                            {
                                var newValue = value.ReplaceIgnoreCase(relPath, "$(ProjectPath)");
                                include.SetValue(newValue);
                            }
                        }
                        else if (value.StartsWithIgnoreCase(relPath))
                        {
                            var newValue = value.ReplaceIgnoreCase(relPath, "$(ProjectPath)");
                            include.SetValue(newValue);
                        }
                        else if (value.StartsWith("..\\"))
                        {
                            var newValue = "$(ProjectPath)\\" + value;
                            include.SetValue(newValue);
                        }
                    }
                }

                // remove empty group
                file.Document.GetAll(Tags.PropertyGroup).ToList()
                             .ForEach(group => group.RemoveIfEmpty());
                file.Document.GetAll(Tags.ItemGroup).ToList()
                             .ForEach(group => group.RemoveIfEmpty());

                // save
                file.Save(omniDeclaration: true);
            }
            catch (Exception e)
            {
                ConsoleLog.Debug(oldFile);
                ConsoleLog.Debug(fullPath);
                ConsoleLog.Debug(e);
            }
        }

        private static void CreateDirsFile(string folder, bool isRoot = false)
        {
            string[] files = Directory.GetFiles(folder, "*proj");
            if (files.Any())
            {
                return;
            }
            var subFolders = Directory.GetDirectories(folder);
            if (subFolders.Count() > 0)
            {
                string dirsFile = Path.Combine(folder, "dirs");
                var lines = new List<string>();
                string header = isRoot ? @"OPTIONAL_DIRS = \" : @"DIRS = \";
                lines.Add(header);
                foreach (var subFolder in subFolders)
                {
                    string folderName = subFolder.Split('\\').Last();
                    lines.Add($"    {folderName} \\");
                    CreateDirsFile(subFolder);
                }
                File.WriteAllLines(dirsFile, lines);
            }
        }

    }
}

namespace Ceres
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Xml.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Mint.Common.Extensions;
    using Mint.Common.Utilities;
    using Mint.Substrate;
    using Mint.Substrate.Construction;

    public static class MoveExecutor
    {
        private static string FastSrcDir = CeresSettings.Settings.FastRepoSrcDir;

        public static List<WaveItem> LoadPlan()
        {
            string json = FileUtils.ReadJson(Files.move_plan);
            return JsonSerializer.Deserialize<List<WaveItem>>(json);
        }

        internal static void MoveWave(WaveItem wave)
        {
            var sourceMap = AnalysisTypeSources(wave.Types);
            var fileCopied = new List<string>();
            foreach (var group in sourceMap)
            {
                string fastFullNamespace = group.Key;
                var sourceFiles = group.Value;
                var meta = new TypeSubMeta(Repo.Paths.SrcDir, fastFullNamespace);

                string buildFile = GetOrCreateBuildFile(meta);
                fileCopied.AddRange(CopySourceFiles(buildFile, meta, sourceFiles));
            }
            ConsoleLog.Ignore("----------------------------------------------------------------");
            ConsoleLog.Warning("File Copied:");
            ConsoleLog.Ignore("----------------------------------------------------------------");
            fileCopied.ForEach(f => ConsoleLog.Highlight(f));
        }

        private static Dictionary<string, List<string>> AnalysisTypeSources(List<string> fullWaveTypes)
        {
            var sources = new Dictionary<string, List<string>>();

            string json = FileUtils.ReadJson(Files.ceres_csproj_path);
            var projDict = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json);

            // 1. group full wave types by assembly to reduce search time
            var assemblyGroup = fullWaveTypes.GroupBy(c => c.Split(',')[0]);
            // 2. foreach assembly in this wave, we cache a map <type -> sourcefile> for feature use
            foreach (var group in assemblyGroup)
            {
                string assemblyName = Path.GetFileNameWithoutExtension(group.Key);
                if (assemblyName.Equals("Microsoft.Ceres.SearchCore.FastServer.DumpLib.Managed"))
                {
                    continue;
                }
                var typeCache = CacheTypeSourceMapByAssembly(projDict, assemblyName);
                foreach (var fullTypeName in group)
                {
                    var parts = fullTypeName.Split(',');
                    var nsType = parts[1] + ',' + parts[2];
                    if (!typeCache.ContainsKey(nsType))
                    {
                        ConsoleLog.Error($"Failed to find type {nsType}.");
                    }
                    else
                    {
                        sources[fullTypeName] = typeCache[nsType];
                    }
                }
            }
            return sources;
        }

        private static string GetOrCreateBuildFile(TypeSubMeta meta)
        {
            var buildFile = meta.SubPorjectFile;
            if (!File.Exists(buildFile))
            {
                var template = FileUtils.ReadText(Files.project_template);
                FileUtils.CreateAndWriteLines(buildFile, template);

                var file = Repo.Load<BuildFile>(buildFile);
                file.Document.GetFirst(Tags.ProjectGuid).SetValue(Guid.NewGuid());
                file.Document.GetFirst(Tags.AssemblyName).SetValue(meta.SubAssemblyName);
                file.Save();
            }
            return buildFile;
        }

        private static List<string> CopySourceFiles(string buildFile, TypeSubMeta meta, List<string> sourceFiles)
        {
            var fileCopied = new List<string>();
            var file = Repo.Load<BuildFile>(buildFile);
            var compileGroup = file.Document.GetFirst(Tags.Compile).Parent; // Assert it exists
            foreach (var srcFile in sourceFiles)
            {
                try
                {
                    string fileName = Path.GetFileName(srcFile);
                    string dstFile = Path.Combine(meta.SubSourceFolder, fileName);
                    if (File.Exists(dstFile))
                    {
                        continue;
                    }
                    FileUtils.CopyFile(srcFile, dstFile);
                    compileGroup.Add(MakeCompileElement(meta.SubRootFolder, dstFile));
                    compileGroup.RemoveNamespace();
                    fileCopied.Add(dstFile);
                }
                catch (Exception e)
                {
                    ConsoleLog.Error($"Fail to move source file: '{srcFile}'.\n{e.Message}");
                }
            }
            compileGroup.SortByAttribute(Tags.Include);
            file.Save();
            return fileCopied;
        }

        private static XElement MakeCompileElement(string folder, string sourceFile)
        {
            string relativePath = PathUtils.GetRelativePath(folder, sourceFile);
            var element = new XElement(Tags.Compile);
            element.SetAttributeValue(Tags.Include, relativePath);
            return element;
        }

        private static Dictionary<string, List<string>> CacheTypeSourceMapByAssembly(Dictionary<string, List<string>> projDict, string assmelbyName)
        {
            var typeCache = new Dictionary<string, List<string>>();
            if (!projDict.ContainsKey(assmelbyName))
            {
                throw new Exception($"Unknown ceres project assembly name: '{assmelbyName}'");
            }
            var csprojs = projDict[assmelbyName];
            csprojs.ForEach(c => ConsoleLog.Warning($"{assmelbyName} -> csproj: {c}"));
            foreach (var csproj in csprojs)
            {
                string projectPath = Path.Combine(FastSrcDir, csproj);
                string projectFolder = Directory.GetParent(projectPath).FullName;
                var compiles = Repo.Load<BuildFile>(projectPath).Document.GetAll(Tags.Compile);
                foreach (var compile in compiles)
                {
                    string include = compile.GetAttribute(Tags.Include).Value;
                    string sourceFile = Path.Combine(projectFolder, include);
                    if (!File.Exists(sourceFile))
                    {
                        // ConsoleLog.Error($"Source File: {sourceFile} doesn't exists.");
                        continue;
                    }
                    var sourceType = GetSourceCodeTypesBySourceFile(sourceFile);
                    foreach (var sType in sourceType)
                    {
                        string typeName = sType.Key;
                        var sources = sType.Value;
                        if (!typeCache.ContainsKey(typeName))
                        {
                            typeCache[typeName] = new List<string>();
                        }
                        typeCache[typeName].AddRange(sources);
                    }
                }
            }
            return typeCache;
        }

        private static Dictionary<string, List<string>> GetSourceCodeTypesBySourceFile(string sourceFile)
        {
            var typeMap = new Dictionary<string, List<string>>();
            SyntaxTree tree = CSharpSyntaxTree.ParseText(File.ReadAllText(sourceFile));
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            foreach (NamespaceDeclarationSyntax _namespace in root.Members)
            {
                string ns = _namespace.Name.ToString();
                foreach (var member in _namespace.Members)
                {
                    string identifier = "";
                    switch (member.Kind())
                    {
                        case SyntaxKind.DelegateDeclaration:
                            identifier = ((DelegateDeclarationSyntax) member).Identifier.ToString();
                            break;
                        case SyntaxKind.NamespaceDeclaration:
                            break;
                        default:
                            identifier = ((BaseTypeDeclarationSyntax) member).Identifier.ToString();
                            break;
                    }
                    string typeName = ns + "," + identifier;
                    if (!typeMap.ContainsKey(typeName))
                    {
                        typeMap[typeName] = new List<string>();
                    }
                    typeMap[typeName].Add(sourceFile);
                }
            }
            return typeMap;
        }

    }
}

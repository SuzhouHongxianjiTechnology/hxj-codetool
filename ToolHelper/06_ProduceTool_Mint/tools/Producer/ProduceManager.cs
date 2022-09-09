namespace Producer
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Mint.Common.Extensions;
    using Mint.Substrate;
    using Mint.Substrate.Construction;
    using Mint.Substrate.Porting;

    internal class ProduceManager
    {
        private readonly LookupTable lookupTable;

        private readonly PortingMetadata metadata;

        internal BuildFileProducer producer { get; }
        internal BuildFileFormater formater { get; }

        internal ProduceManager(LookupTable lookupTable, string buildFilePath, string framework)
        {
            this.lookupTable = lookupTable;
            this.metadata = new PortingMetadata(buildFilePath, framework);

            var resolver = new ReferenceResolver(metadata.NFBuildFile, this.lookupTable);
            var converter = new ElementConverter(metadata, this.lookupTable, resolver);

            this.producer = new BuildFileProducer(converter);
            this.formater = new BuildFileFormater(this.lookupTable);
        }

        internal void ResetRestoreEntry()
        {
            var restoreEntry = Repo.RestoreEntry;

            string nsPath = this.metadata.NSBuildFile.ReplaceIgnoreCase(Repo.Paths.SrcDir, "$(Inetroot)");
            restoreEntry.RemovePath(nsPath);
            restoreEntry.Save();

            string ncPath = this.metadata.NCBuildFile.ReplaceIgnoreCase(Repo.Paths.SrcDir, "$(Inetroot)");
            restoreEntry.RemovePath(ncPath);
            restoreEntry.Save();
        }

        internal void ResetDirsProj()
        {
            var dirsProj = Repo.LoadTextDirsFile(this.metadata.ProducedDirsProj);

            string nsDirs = this.metadata.NSBuildFolderName;
            dirsProj.RemovePath(nsDirs);
            dirsProj.Save();

            string ncDirs = this.metadata.NCBuildFolderName;
            dirsProj.RemovePath(ncDirs);
            dirsProj.Save();
        }

        internal void SetupBuildFile()
        {
            if (Directory.Exists(this.metadata.NSBuildFolder))
                Directory.Delete(this.metadata.NSBuildFolder, recursive: true);

            if (Directory.Exists(this.metadata.NCBuildFolder))
                Directory.Delete(this.metadata.NCBuildFolder, recursive: true);

            Directory.CreateDirectory(this.metadata.ProducedFolder);
            File.Copy(this.metadata.NFBuildFile, this.metadata.ProducedFile);
        }

        internal void CleanupBuildFile()
        {
            if (Directory.Exists(this.metadata.NSBuildFolder))
                Directory.Delete(this.metadata.NSBuildFolder, recursive: true);

            if (Directory.Exists(this.metadata.NCBuildFolder))
                Directory.Delete(this.metadata.NCBuildFolder, recursive: true);
        }

        internal void ConvertBuildFile()
        {
            var buildFile = Repo.Load<BuildFile>(this.metadata.ProducedFile);
            this.producer.Produce(buildFile);
            this.formater.Format(buildFile);
            buildFile.Save(omniDeclaration: true);
        }

        internal void AddPathToRestoreEntry()
        {
            var restoreEntry = Repo.RestoreEntry;
            string path = this.metadata.ProducedFile.ReplaceIgnoreCase(Repo.Paths.SrcDir, "$(Inetroot)");
            restoreEntry.AddPath(path);
            restoreEntry.OrganizeProjects();
            restoreEntry.Save();
        }

        internal void AddPathToDirsProj()
        {
            var dirsProj = Repo.LoadTextDirsFile(this.metadata.ProducedDirsProj);
            string anchor = this.metadata.NFBuildFolderName;
            string path = this.metadata.ProducedFolderName;
            dirsProj.AddPathAfter(anchor, path);
            dirsProj.Save();
        }

        internal (List<string>, List<string>) FindBlockAndUndefined()
        {
            var buildFile = Repo.Load<BuildFile>(this.metadata.ProducedFile);

            List<string> blocked = buildFile.Document
                                            .GetAll(Tags.Reference)
                                            .Where(r => r.HasAttribute(Tags.Blocked))
                                            .Select(r => r.GetAttribute(Tags.Include).Value)
                                            .ToList();

            List<string> undefined = buildFile.Document
                                              .GetAll(Tags.PackageReference)
                                              .Where(p => p.HasAttribute(Tags.Undefined))
                                              .Select(p => p.GetAttribute(Tags.Include).Value)
                                              .ToList();

            return (blocked, undefined);
        }
    }
}

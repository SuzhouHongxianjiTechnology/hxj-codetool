namespace ProduceTool
{
    using System.IO;
    using System.Linq;
    using Mint.Common;
    using Mint.Substrate.Construction;
    using Mint.Substrate.Production;
    using Mint.Substrate.Utilities;

    internal class ProduceManager
    {
        internal const string RestoreEntry = @"sdkProjectRestoreEntry\dirs.proj";

        internal const string PackagesProps = @"Packages.props";

        internal const string InnerCorextConfig = @"build\corext\corext.config";

        internal const string OuterCorextConfig = @".corext\corext.config";

        internal const string VariantConfiguration = @"build\msbuild\targets\VariantConfiguration\VariantConfiguration.targets";

        private string srcDir;

        private PortingConfig config;

        private RestoreEntryFile restoreEntry;

        private DirsProjFile dirsProj;

        internal ProduceManager(string srcDir, PortingConfig config)
        {
            this.srcDir = srcDir;
            this.config = config;
            this.restoreEntry = new RestoreEntryFile(Path.Combine(srcDir, RestoreEntry));
            this.dirsProj = new DirsProjFile(config.ProjectDirs);
        }

        // ------------------------------------------------------------

        internal void ResetRestoreEntry()
        {
            using (this.restoreEntry)
            {
                string netStdStr = SubstrateUtils.ToInetrootPath(this.srcDir, this.config.NetStdFilePath);
                restoreEntry.RemovePath(netStdStr);

                string netCoreStr = SubstrateUtils.ToInetrootPath(this.srcDir, this.config.NetCoreFilePath);
                restoreEntry.RemovePath(netCoreStr);
            }
        }

        internal void ProduceRestoreEntry()
        {
            using (this.restoreEntry)
            {
                string newValue = SubstrateUtils.ToInetrootPath(this.srcDir, this.config.ProduceFilePath);
                restoreEntry.AddPath(newValue);
            }
        }

        // ------------------------------------------------------------

        internal void ResetDirsProj()
        {
            using (this.dirsProj)
            {
                string netStdDir = config.NetStdParentName + @"\*.*proj";
                this.dirsProj.RemovePath(netStdDir);

                string netCoreDir = config.NetCoreParentName + @"\*.*proj";
                this.dirsProj.RemovePath(netCoreDir);
            }
        }

        internal void ProduceDirsProj()
        {
            using (this.dirsProj)
            {
                string newValue = config.ProduceParentName + @"\*.*proj";
                this.dirsProj.AddPath(newValue);
            }
        }

        // ------------------------------------------------------------

        internal void ProduceProject()
        {
            if (Directory.Exists(this.config.NetStdParentPath))
            {
                Directory.Delete(this.config.NetStdParentPath, recursive: true);
            }

            if (Directory.Exists(this.config.NetCoreParentPath))
            {
                Directory.Delete(this.config.NetCoreParentPath, recursive: true);
            }

            Directory.CreateDirectory(this.config.ProduceParentPath);
            File.Copy(this.config.NetFrameworkFilePath, this.config.ProduceFilePath);

            using (var file = new PortableProjectFile(this.config.ProduceFilePath))
            {
                file.Produce(this.config);
            }
        }

        internal void FormatProjectFile()
        {
            using (var file = new NetCoreProjectFile(this.config.ProduceFilePath))
            {
                file.Format();
            }
        }

        internal void AnalyizeProject()
        {
            var producedFile = new NetCoreProjectFile(this.config.ProduceFilePath);
            var blocked = producedFile.References
                                      .Where(r => r.IsBlocked)
                                      .Select(r => r.Name)
                                      .ToList();
            var undefined = producedFile.PackageReferences
                                        .Where(p => p.IsUndefined)
                                        .Select(p => p.Name)
                                        .ToList();

            if (blocked.Any())
            {
                ConsoleLog.Error($"Blocked by Substrate project(s):");
                blocked.ForEach(b => ConsoleLog.Error($"  {b}"));
            }

            if (undefined.Any())
            {
                ConsoleLog.Error($"Undedined package(s) used:");
                undefined.ForEach(u => ConsoleLog.Error($"  {u}"));
            }
        }
    }
}

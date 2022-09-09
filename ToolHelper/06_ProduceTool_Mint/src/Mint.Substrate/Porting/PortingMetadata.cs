namespace Mint.Substrate.Porting
{
    using System.IO;
    using Mint.Substrate.Construction;

    public class PortingMetadata
    {
        private const string NetStd  = ".NetStd";
        private const string NetCore = ".NetCore";

        private readonly string framework;

        private readonly string nfBuildFolder;
        private readonly string nfBuildFolderName;
        private readonly string nfBuildFile;
        private readonly string nfBuildFileName;

        private readonly string nsBuildFolder;
        private readonly string nsBuildFolderName;
        private readonly string nsBuildFile;
        private readonly string nsBuildFileName;

        private readonly string ncBuildFolder;
        private readonly string ncBuildFolderName;
        private readonly string ncBuildFile;
        private readonly string ncBuildFileName;

        private readonly string producedFolder;
        private readonly string producedFolderName;
        private readonly string producedFile;
        private readonly string producedFileName;

        private readonly string producedDirsProj;

        public PortingMetadata(string nfBuildFile, string framework)
        {
            string extension = Path.GetExtension(nfBuildFile);

            this.framework = framework;

            this.nfBuildFolderName = Directory.GetParent(nfBuildFile).Name;
            this.nfBuildFolder     = Directory.GetParent(nfBuildFile).FullName;
            this.nfBuildFileName   = Path.GetFileName(nfBuildFile);
            this.nfBuildFile       = nfBuildFile;

            string nfBuildFileNameWithoutExtension = Path.GetFileNameWithoutExtension(nfBuildFile);

            this.nsBuildFolderName = $"{this.nfBuildFolderName}{NetStd}";
            this.nsBuildFolder     = $"{this.nfBuildFolder}{NetStd}";
            this.nsBuildFileName   = $"{nfBuildFileNameWithoutExtension}{NetStd}{extension}";
            this.nsBuildFile       = $"{this.nsBuildFolder}\\{this.nsBuildFileName}";

            this.ncBuildFolderName = $"{this.nfBuildFolderName}{NetCore}";
            this.ncBuildFolder     = $"{this.nfBuildFolder}{NetCore}";
            this.ncBuildFileName   = $"{nfBuildFileNameWithoutExtension}{NetCore}{extension}";
            this.ncBuildFile       = $"{this.ncBuildFolder}\\{this.ncBuildFileName}";

            this.producedFolder     = framework == Frameworks.NetStd ? this.nsBuildFolder     : this.ncBuildFolder;
            this.producedFolderName = framework == Frameworks.NetStd ? this.nsBuildFolderName : this.ncBuildFolderName;
            this.producedFile       = framework == Frameworks.NetStd ? this.nsBuildFile       : this.ncBuildFile;
            this.producedFileName   = framework == Frameworks.NetStd ? this.nsBuildFileName   : this.ncBuildFileName;

            this.producedDirsProj = Path.Combine(Directory.GetParent(this.nfBuildFolder).ToString(), "dirs");
        }

        public string Framework          => this.framework;
        public string NFBuildFolder      => this.nfBuildFolder;
        public string NFBuildFolderName  => this.nfBuildFolderName;
        public string NFBuildFile        => this.nfBuildFile;
        public string NFBuildFileName    => this.nfBuildFileName;
        public string NSBuildFolder      => this.nsBuildFolder;
        public string NSBuildFolderName  => this.nsBuildFolderName;
        public string NSBuildFile        => this.nsBuildFile;
        public string NSBuildFileName    => this.nsBuildFileName;
        public string NCBuildFolder      => this.ncBuildFolder;
        public string NCBuildFolderName  => this.ncBuildFolderName;
        public string NCBuildFile        => this.ncBuildFile;
        public string NCBuildFileName    => this.ncBuildFileName;
        public string ProducedFolder     => this.producedFolder;
        public string ProducedFolderName => this.producedFolderName;
        public string ProducedFile       => this.producedFile;
        public string ProducedFileName   => this.producedFileName;
        public string ProducedDirsProj   => this.producedDirsProj;
    }
}

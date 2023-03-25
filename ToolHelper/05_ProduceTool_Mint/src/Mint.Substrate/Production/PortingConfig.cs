namespace Mint.Substrate.Production
{
    using System.IO;

    public class PortingConfig
    {
        private const string NetStd = ".NetStd";
        private const string NetCore = ".NetCore";

        public string TargetFramework { get; }

        // NetFramework Project
        public string NetFrameworkFileName { get; }
        public string NetFrameworkParentPath { get; }
        public string NetFrameworkParentName { get; }
        public string NetFrameworkFilePath { get; }

        // NetStd Project
        public string NetStdFileName { get; }
        public string NetStdParentPath { get; }
        public string NetStdParentName { get; }
        public string NetStdFilePath { get; }

        // NetCore Project
        public string NetCoreFileName { get; }
        public string NetCoreParentPath { get; }
        public string NetCoreParentName { get; }
        public string NetCoreFilePath { get; }

        // Produce Type
        public string ProduceFileName { get; }
        public string ProduceParentPath { get; }
        public string ProduceParentName { get; }
        public string ProduceFilePath { get; }

        // Other Path
        public string ProjectDirs { get; }

        public static PortingConfig Create(string netFrameworkPath, string framework)
        {
            return new PortingConfig(netFrameworkPath, framework);
        }

        private PortingConfig(string filePath, string framework)
        {
            string extension = Path.GetExtension(filePath);

            this.TargetFramework = framework;
            this.NetFrameworkFilePath = filePath;

            this.NetFrameworkFileName   = Path.GetFileNameWithoutExtension(this.NetFrameworkFilePath);
            this.NetFrameworkParentPath = Directory.GetParent(this.NetFrameworkFilePath).ToString();
            this.NetFrameworkParentName = Directory.GetParent(this.NetFrameworkFilePath).Name;

            // NetStd Project
            this.NetStdFileName   = string.Concat(NetFrameworkFileName, NetStd);
            this.NetStdParentPath = string.Concat(NetFrameworkParentPath, NetStd);
            this.NetStdParentName = string.Concat(NetFrameworkParentName, NetStd);
            this.NetStdFilePath   = string.Concat(NetStdParentPath, "\\", NetStdFileName, extension);

            // NetCore Project
            this.NetCoreFileName   = string.Concat(NetFrameworkFileName, NetCore);
            this.NetCoreParentPath = string.Concat(NetFrameworkParentPath, NetCore);
            this.NetCoreParentName = string.Concat(NetFrameworkParentName, NetCore);
            this.NetCoreFilePath   = string.Concat(NetCoreParentPath, "\\", NetCoreFileName, extension);

            // Produce Type
            this.ProduceFileName   = TargetFramework == Construction.TargetFramework.NetStd ? NetStdFileName : NetCoreFileName;
            this.ProduceParentPath = TargetFramework == Construction.TargetFramework.NetStd ? NetStdParentPath : NetCoreParentPath;
            this.ProduceParentName = TargetFramework == Construction.TargetFramework.NetStd ? NetStdParentName : NetCoreParentName;
            this.ProduceFilePath   = TargetFramework == Construction.TargetFramework.NetStd ? NetStdFilePath : NetCoreFilePath;

            // Other Path
            this.ProjectDirs = Path.Combine(Directory.GetParent(NetFrameworkParentPath).ToString(), @"dirs.proj");
        }
    }
}

namespace Mint.Substrate.Construction
{
    using Mint.Substrate.Utilities;

    public class Project : IProject
    {
        public string Name { get; }

        public string FilePath { get; }

        public ProjectType Type { get; }

        public string Framework { get; }

        public bool IsProduced { get; }

        public string? TargetPath { get; }

        public Project(string name, string framework, string filePath, ProjectType type)
        {
            this.Name = name;

            this.FilePath = filePath;

            this.Type = type;

            this.Framework = framework;

            this.IsProduced = (this.Framework == Frameworks.NetStd || this.Framework == Frameworks.NetCore || this.Framework == Frameworks.Net) || this.Type == ProjectType.CPP;

            bool hasTargetPath = this.Type == ProjectType.CPP ||
                                 this.Type == ProjectType.Substrate;

            if (this.IsProduced && hasTargetPath)
            {
                this.TargetPath = MSBuildUtils.ResolveTargetPath(name, filePath);
            }
        }
    }
}

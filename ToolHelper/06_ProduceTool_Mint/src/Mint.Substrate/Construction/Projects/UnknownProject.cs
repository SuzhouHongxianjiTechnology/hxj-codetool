namespace Mint.Substrate.Construction
{
    using System.IO;
    using Mint.Substrate.Utilities;

    public class UnknownProject : IProject
    {
        public string Name { get; }

        public string FilePath { get; }

        public ProjectType Type => ProjectType.Unknown;

        public string Framework { get; }

        public bool IsProduced => false;

        public string? TargetPath { get; }

        public UnknownProject(string filePath)
        {
            this.Name = Path.GetFileNameWithoutExtension(filePath);
            this.FilePath = filePath;
            this.Framework = MSBuildUtils.InferFrameworkByPath(filePath);
        }
    }
}

namespace Mint.Substrate.Utilities
{
    using System.IO;
    using System.Linq;
    using Mint.Common.Extensions;

    internal sealed class MSBuildPath
    {
        internal string FullPath { get; }

        internal string MsBuildProjectDirectory { get; }

        internal string SourcesRootDir { get; }

        internal string InetRoot { get; }

        internal string Root { get; }

        internal string CompDir { get; }

        internal string CompDirSrc { get; }

        internal MSBuildPath(string path)
        {
            // ErrorUtils.VerifyThrowInvalidBuildPath(path);

            this.FullPath = path;
            this.MsBuildProjectDirectory = path;
            this.SourcesRootDir = path;

            var parts = path.SplitIgnoreCase(@"\src");
            string srcDir = Path.Combine(parts[0], "src"); ;
            this.InetRoot = srcDir;
            this.Root = srcDir;

            string compDir = string.Join(@"\src", parts.Take(parts.Count() - 1));
            compDir = Path.GetFullPath(compDir);
            this.CompDir = compDir;

            this.CompDirSrc = Path.GetFullPath(Path.Combine(compDir, "src"));
        }
    }
}

namespace Mint.Substrate.Utilities
{
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Mint.Common.Extensions;
    using Mint.Common.Utilities;
    using Mint.Substrate.Construction;

    public static class MSBuildUtils
    {
        public static string GetRelativePath(string thisParent, string targetParent, string oldPath)
        {
            string fullPath;

            if (MSBuildUtils.TryResolveBuildVariables(thisParent, oldPath, out fullPath))
            {
                return PathUtils.GetRelativePath(targetParent, fullPath);
            }

            // cannot resolve this variable, just use it.
            if (oldPath.StartsWith("$(") || oldPath.StartsWith("@("))
            {
                return oldPath;
            }

            fullPath = PathUtils.GetAbsolutePath(thisParent, oldPath);
            return PathUtils.GetRelativePath(targetParent, fullPath);
        }

        public static bool TryResolveBuildVariables(string thisParent, string rawValue, out string result)
        {
            var msBuildPath = new MSBuildPath(thisParent);

            if (rawValue.StartsWithIgnoreCase("$(CompDirSrc)"))
            {
                result = rawValue.ReplaceIgnoreCase("$(CompDirSrc)", msBuildPath.CompDirSrc);
                return true;
            }

            if (rawValue.StartsWithIgnoreCase("$(CompDir)"))
            {
                result = rawValue.ReplaceIgnoreCase("$(CompDir)", "");
                result = Path.Combine(msBuildPath.CompDir, result);
                return true;
            }

            if (rawValue.StartsWithIgnoreCase("$(InetRoot)"))
            {
                result = rawValue.ReplaceIgnoreCase("$(InetRoot)", msBuildPath.InetRoot);
                return true;
            }

            if (rawValue.StartsWithIgnoreCase("$(MsBuildProjectDirectory)"))
            {
                result = rawValue.ReplaceIgnoreCase("$(MsBuildProjectDirectory)", "");
                result = Path.Combine(msBuildPath.MsBuildProjectDirectory, result);
                return true;
            }

            if (rawValue.StartsWithIgnoreCase("$(Root)"))
            {
                result = rawValue.ReplaceIgnoreCase("$(Root)", "");
                result = Path.Combine(msBuildPath.Root, result);
                return true;
            }

            if (rawValue.StartsWithIgnoreCase("$(SourcesRootDir)"))
            {
                result = rawValue.ReplaceIgnoreCase("$(SourcesRootDir)", "");
                result = Path.Combine(msBuildPath.SourcesRootDir, result);
                return true;
            }

            result = rawValue;
            return false;
        }

        public static string InferNFBuildFileByPath(string producedFile)
        {
            producedFile = producedFile.ReplaceIgnoreCase(@".NetCore", "");
            producedFile = producedFile.ReplaceIgnoreCase(@".NetStd", "");
            return producedFile;
        }

        public static string InferFrameworkByPath(string producedFile)
        {
            string parent = Directory.GetParent(producedFile).ToString();
            string suffix = parent.Split(".").Last();
            switch (suffix.ToLower())
            {
                case "netcore":
                    return Frameworks.NetCore;
                case "netstd":
                    return Frameworks.NetStd;
                default:
                    return Frameworks.NetFramework;
            }
        }

        public static string ResolveTargetPath(string assemblyName, string fullPath)
        {
            // full path:
            // eg: D:\xxx\xxx\sources\dev\assistants\src\Assistants.NetCore\Microsoft.Exchange.Assistants.NetCore.csproj

            // dev\assistants\src\Assistants.NetCore\Microsoft.Exchange.Assistants.NetCore.csproj
            string devPath = fullPath.Split(@"sources\").Last();

            // dev\assistants\
            string rootPath = Regex.Split(devPath, "src", RegexOptions.IgnoreCase).First();

            // Microsoft.Exchange.Assistants.NetCore
            string folderName = Path.GetFileNameWithoutExtension(fullPath);

            // $(TargetPathDir)dev\assistants\Microsoft.Exchange.Assistants.NetCore\$(FlavorPlatformDir)\{assemblyName}.dll
            return $"$(TargetPathDir){rootPath}{folderName}\\$(FlavorPlatformDir)\\{assemblyName}.dll";
        }
    }
}

namespace Mint.Substrate.Utilities
{
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Mint.Common;
    using Mint.Substrate.Construction;

    public static class SubstrateUtils
    {
        private static Regex pkgRegex = new Regex(@"\$\(Pkg(.*?)([_\d]*)\).*");

        // private static Regex varRegex = new Regex(@"\$\(.+?\)");

        public static string ResolvePackageName(string text)
        {
            MatchCollection matchCollection = pkgRegex.Matches(text);
            var pkgname = matchCollection[0].Groups[1].Value.Replace("_", ".");

            if (string.Equals("RPSv", pkgname))
            {
                pkgname = "RPSv6.7";
            }

            return pkgname;
        }

        public static string ResolveVariables(string parent, string path)
        {
            if (StringUtils.ContainsIgnoreCase(path, "$(COMPDIRSRC)"))
            {
                return StringUtils.ReplaceIgnoreCase(path, "$(COMPDIRSRC)", GetCompDirSrc(parent));
            }

            if (StringUtils.ContainsIgnoreCase(path, "$(MsBuildProjectDirectory)"))
            {
                return StringUtils.ReplaceIgnoreCase(path, "$(MsBuildProjectDirectory)", parent);
            }

            if (StringUtils.ContainsIgnoreCase(path, "$(SourcesRootDir)"))
            {
                return StringUtils.ReplaceIgnoreCase(path, "$(SourcesRootDir)", string.Empty);
            }

            if (StringUtils.ContainsIgnoreCase(path, "$(InetRoot)"))
            {
                return StringUtils.ReplaceIgnoreCase(path, "$(InetRoot)", parent.Split(@"\sources\").First());
            }

            if (StringUtils.ContainsIgnoreCase(path, "$(ROOT)"))
            {
                return StringUtils.ReplaceIgnoreCase(path, "$(ROOT)", parent.Split(@"\sources\").First());
            }

            return path;
        }

        public static string ResolveItemFullPath(string root, string path)
        {
            string src = root.Split(@"\sources\").First();

            if (path.StartsWith("$"))
            {
                path = SubstrateUtils.ResolveVariables(root, path);
                if (path.StartsWith("$") || path.StartsWith("@"))
                {
                    return path;
                }
            }

            if (StringUtils.StartsWithIgnoreCase(path, root))
            {
                return path;
            }

            if (StringUtils.StartsWithIgnoreCase(path, @"sources\"))
            {
                return Path.Combine(src, path);
            }

            return Path.GetFullPath(Path.Combine(root, path));
        }

        public static string GetCompDirSrc(string baseDir)
        {
            var parent = Directory.GetParent(baseDir);
            while (parent != null)
            {
                if (StringUtils.EqualsIgnoreCase("src", parent.Name))
                {
                    return parent.FullName;
                }
                parent = Directory.GetParent(parent.FullName);
            }
            return string.Empty;
        }

        public static string ToInetrootPath(string root, string path)
        {
            Verification.VerifyThrowInvalidRootPath(root, path);
            return path.Replace(root, "$(Inetroot)");
        }

        public static string ToSourcePath(string root, string path)
        {
            Verification.VerifyThrowInvalidRootPath(root, path);
            return path.Replace(root + "\\", "");
        }

        public static bool TryFindProducedFile(string netFrameworkPath, out string producePath)
        {
            producePath = SubstrateUtils.InferPathBySuffix(netFrameworkPath, ".netcore") ??
                          SubstrateUtils.InferPathBySuffix(netFrameworkPath, ".netstd");
            return !string.IsNullOrEmpty(producePath);
        }

        public static bool TryFindNetFrameworkFile(string path, out string projectPath)
        {
            path = StringUtils.ReplaceIgnoreCase(path, @".NetCore", "");
            path = StringUtils.ReplaceIgnoreCase(path, @".NetStd", "");
            projectPath = File.Exists(path) ? path : null;
            return File.Exists(projectPath);
        }

        public static string GetFrameworkByPath(string producePath)
        {
            string parent = Directory.GetParent(producePath).ToString();
            string suffix = parent.Split(".").Last();
            switch (suffix.ToLower())
            {
                case "netcore":
                    return TargetFramework.NetCore;
                case "netstd":
                    return TargetFramework.NetStd;
                default:
                    return TargetFramework.NetFramework;
            }
        }

        // ----------------------------------------------------------------

        private static string InferPathBySuffix(string netFrameworkPath, string suffix)
        {
            string fxParent = Directory.GetParent(netFrameworkPath).ToString();
            string fxFileName = Path.GetFileNameWithoutExtension(netFrameworkPath);

            string targetParent = string.Concat(fxParent, suffix);
            try
            {
                string filePath = Directory.GetFiles(targetParent, "*.*proj").FirstOrDefault();
                return File.Exists(filePath) ? filePath : null;
            }
            catch
            {
                return null;
            }
        }
    }
}

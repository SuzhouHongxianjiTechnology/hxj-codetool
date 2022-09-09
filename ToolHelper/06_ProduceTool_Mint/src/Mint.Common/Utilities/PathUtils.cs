namespace Mint.Common
{
    using System.IO;
    using System.Reflection;

    public static class PathUtils
    {
        public static string ApplicationRoot()
        {
            string thisPath = Assembly.GetEntryAssembly().Location;
            return Directory.GetParent(thisPath).ToString();
        }

        public static string GetAbsolutePath(string parent, string path)
        {
            path = path.StartsWith("\\") ? path.Substring(1) : path;
            return Path.GetFullPath(Path.Combine(parent, path));
        }

        public static string GetRelativePath(string parent, string path)
        {
            return Path.GetRelativePath(parent, path);
        }
    }
}

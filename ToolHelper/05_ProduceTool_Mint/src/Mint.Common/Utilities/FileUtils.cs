namespace Mint.Common
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public static class FileUtils
    {
        public static List<string> ReadLines(string fileName)
        {
            string filePath = FileFullPath(fileName, "txt");
            return File.ReadLines(filePath)
                       .Where(x => !string.IsNullOrEmpty(x.Trim()))
                       .Where(x => !x.StartsWith("#"))
                       .ToList();
        }

        public static void WriteLines(string fileName, IEnumerable<string> lines)
        {
            string filePath = FileFullPath(fileName, "txt");
            File.WriteAllLines(filePath, lines);
        }

        public static void WriteJson(string fileName, string json)
        {
            string filePath = FileFullPath(fileName, "json");
            File.WriteAllText(filePath, json);
        }

        public static string ReadJson(string fileName)
        {
            string filePath = FileFullPath(fileName, "json");
            return File.ReadAllText(filePath);
        }

        private static string FileFullPath(string fileName, string extension)
        {
            fileName = StringUtils.EndsWithIgnoreCase(fileName, extension) ? fileName
                                                                           : $"{fileName}.{extension}";
            return Path.Combine(PathUtils.ApplicationRoot(), fileName);
        }
    }
}

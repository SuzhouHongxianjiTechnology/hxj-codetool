namespace Mint.Common.Utilities
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Mint.Common.Extensions;

    public static class FileUtils
    {
        /// <summary>
        /// Create a file.
        /// </summary>
        public static FileStream Create(string path)
        {
            var dir = Directory.GetParent(path).FullName;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return File.Create(path);
        }

        /// <summary>
        /// Create a file (if not exists) and write lines to in it as a txt file.
        /// </summary>
        public static void CreateAndWriteLines(string path, IEnumerable<string> lines)
        {
            if (!File.Exists(path))
            {
                var dir = Directory.GetParent(path).FullName;
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                using (StreamWriter writer = File.CreateText(path))
                {
                    foreach (var line in lines)
                    {
                        writer.WriteLine(line);
                    }
                }
            }
            else
            {
                File.WriteAllLines(path, lines);
            }
        }

        /// <summary>
        /// Copy a file from source to destination.
        /// </summary>
        public static void CopyFile(string from, string to)
        {
            if (!File.Exists(from))
            {
                throw new FileNotFoundException(from);
            }
            var dir = Directory.GetParent(to).FullName;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.Copy(from, to, overwrite: true);
        }

        /// <summary>
        /// Reads all lines from a 'txt' file.
        /// Empty lines and lines that starts with '#' will be ignored.
        /// The search path is the root directory of this assembly.
        /// </summary>
        public static IEnumerable<string> ReadText(string name)
        {
            string path = FileFullPath(name, "txt");
            return File.ReadLines(path)
                       .Where(x => !string.IsNullOrEmpty(x.Trim()));
        }

        /// <summary>
        /// Writes a IEnumerable of lines into a 'txt' file.
        /// The search path is the root directory of this assembly.
        /// </summary>
        public static void WriteText(string name, IEnumerable<string> lines)
        {
            string path = FileFullPath(name, "txt");
            if (!File.Exists(path))
            {
                using (var _ = FileUtils.Create(path))
                { }
            }
            File.WriteAllLines(path, lines);
        }

        /// <summary>
        /// Reads the string content from a 'json' file.
        /// The search path is the root directory of this assembly.
        /// </summary>
        public static string ReadJson(string name)
        {
            string path = FileFullPath(name, "json");
            return File.ReadAllText(path);
        }

        /// <summary>
        /// Writes a string object into a 'json' file.
        /// The search path is the root directory of this assembly.
        /// </summary>
        public static void WriteJson(string name, string json)
        {
            string path = FileFullPath(name, "json");
            if (!File.Exists(path))
            {
                using (var _ = FileUtils.Create(path))
                { }
            }
            File.WriteAllText(path, json);
        }

        /// <summary>
        /// Calculates the full path to a file.
        /// </summary>
        private static string FileFullPath(string name, string extension)
        {
            name = name.EndsWithIgnoreCase(extension)
                        ? name
                        : $"{name}.{extension}";

            return Path.Combine(PathUtils.ApplicationRoot(), name);
        }

        public static T DeserializeJson<T>(string sourcePath)
        {
            string json = FileUtils.ReadJson(sourcePath);
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public static async Task<T> DeserializeJsonAsync<T>(string sourcePath)
        {
            using (FileStream stream = File.OpenRead(sourcePath))
            {
                return await JsonSerializer.DeserializeAsync<T>(stream,
                    new JsonSerializerOptions
                    {
                        MaxDepth = 1024,
                        PropertyNameCaseInsensitive = true,
                    });
            }
        }
    }
}

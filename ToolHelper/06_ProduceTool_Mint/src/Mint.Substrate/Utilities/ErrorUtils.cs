namespace Mint.Substrate.Utilities
{
    using System;
    using System.IO;
    using System.Xml.Linq;
    using Mint.Common.Extensions;

    internal static class ErrorUtils
    {
        internal static void VerifyThrowArgumentNull(object value, string parameterName)
        {
            if (value == null)
            {
                throw new ArgumentNullException($"Value cannot be null. (Parameter '{parameterName}')");
            }
        }

        internal static void VerifyThrowEnvironmentVariableNotDefined(string variableName)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(variableName)))
            {
                throw new InvalidOperationException($"The required environment variable is not defined. (Variable '{variableName}')");
            }
        }

        internal static void VerifyThrowDirectoryNotExists(string dir)
        {
            if (!Directory.Exists(dir))
            {
                throw new DirectoryNotFoundException($"Directory cannot be found. (Directory '{dir}')");
            }
        }

        internal static void VerifyThrowFileNotExists(string? path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"File cannot be found. (File '{path}')");
            }
        }

        internal static void VerifyThrowXmlLoadFail(string path)
        {
            try
            {
                XDocument.Load(path);
            }
            catch (Exception e)
            {
                throw new FileLoadException($"File cannot be loaded as xml file. (File '{path}')" +
                                            $"\nException:\n{e}");
            }
        }

        internal static void VerifyThrowPathIsNotRooted(string path)
        {
            if (!Path.IsPathRooted(path))
            {
                throw new ArgumentException($"Not a rooted path. (Path '{path}')");
            }
        }

        internal static void VerifyThrowInvalidBuildPath(string path)
        {
            path = path.EndsWith(@"\") ? path.Substring(0, path.Length - 1) : path;
            if (!path.ContainsIgnoreCase(@"\src") ||
                 path.EndsWithIgnoreCase(@"\src"))
            {
                throw new InvalidDataException($"Invalid build directory. (Directory {path})");
            }
        }
    }
}

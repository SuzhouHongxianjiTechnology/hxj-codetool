namespace Mint.Substrate.Utilities
{
    using System;
    using System.IO;
    using System.Xml.Linq;
    using Mint.Common;

    internal static class Verification
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

        internal static void VerifyThrowFileNotExists(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"File cannot be found. (Path '{path}')");
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
                throw new FileLoadException($"File cannot be loaded as xml file. (Path '{path}')" + Environment.NewLine +
                                            "Exception:" + Environment.NewLine + $"{e}");
            }
        }

        internal static void VerifyThrowInvalidRootPath(string root, string path)
        {
            if (!Path.IsPathRooted(path) || !StringUtils.StartsWithIgnoreCase(path, root))
            {
                throw new ArgumentException($"Not a valid rooted path. (Path '{path}')");
            }
        }
    }
}

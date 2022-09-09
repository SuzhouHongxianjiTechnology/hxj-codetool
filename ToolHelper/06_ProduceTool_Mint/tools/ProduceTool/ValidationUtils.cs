namespace ProduceTool
{
    using System;
    using System.IO;

    internal static class ValidationUtils
    {
        internal static void VerifyThrowValidNetFrameworkFolder()
        {
            string currentDir = Directory.GetCurrentDirectory();
            if (currentDir.EndsWith("netcore", StringComparison.OrdinalIgnoreCase) ||
                currentDir.EndsWith("netstd", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Cannot produce from directory '{currentDir}'" + Environment.NewLine +
                                                    "It mostly like this is a produced project.");
            }
            var files = Directory.GetFiles(currentDir, "*.csproj");
            if (files.Length != 1)
            {
                throw new InvalidOperationException($"Cannot produce from directory '{currentDir}'" + Environment.NewLine +
                                                    "There should be one and only one 'csproj' file.");
            }
        }
    }
}

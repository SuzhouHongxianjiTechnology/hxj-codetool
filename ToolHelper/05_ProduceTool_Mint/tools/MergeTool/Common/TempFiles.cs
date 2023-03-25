namespace MergeTool.Common
{
    using System.IO;

    internal static class TempFiles
    {

        // basic path
        private static readonly string THIS_PATH = System.Reflection.Assembly.GetEntryAssembly().Location;
        private static readonly string TOOL_PATH = Directory.GetParent(THIS_PATH).ToString();

        // temp files
        public static readonly string NetFrameworkFiles = Path.Combine(TOOL_PATH, @"_NetFrameworkFiles.txt");
        public static readonly string ProducedFiles = Path.Combine(TOOL_PATH, @"_ProducedFiles.txt");
        public static readonly string ChangedFiles = Path.Combine(TOOL_PATH, @"_ChangedFiles.txt");
    }
}

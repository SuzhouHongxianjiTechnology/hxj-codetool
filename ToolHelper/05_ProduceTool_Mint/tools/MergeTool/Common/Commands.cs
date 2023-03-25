namespace MergeTool.Common
{
    using Mint.Common;

    static class Commands
    {
        public static void OpenFileWithCode(string path)
        {
            string command = $@"code {path}";
            Command.Execute(command);
        }

        public static string DiffChangesOnly()
        {
            return string.Format(@"for /f %i in ({0}) do git difftool {1}...{2} -- %i",
                                 TempFiles.ChangedFiles,
                                 Settings.DFCommitID,
                                 Settings.MainCommitID);
        }

        // ----------------------------------------------------------------
        // manually, just in case

        public static string ManuallyScan()
        {
            string someFile = "some_file.txt";
            string command = $@"dir /s /b *.NetCore *.NetStd > {someFile}";
            Command.Execute(command);

            return string.Format(@"for /f %i in ({0}) do git difftool {1}...{2} -- %i",
                                 someFile,
                                 Settings.DFCommitID,
                                 Settings.MainCommitID);
        }
    }
}

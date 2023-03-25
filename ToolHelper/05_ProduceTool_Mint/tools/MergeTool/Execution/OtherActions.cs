namespace MergeTool
{
    using System;
    using MergeTool.Common;
    using Mint.Common;

    internal static class OtherActions
    {
        internal static void OpenSettings()
        {
            Commands.OpenFileWithCode(Settings.FilePath);
        }

        internal static void ShowCommands()
        {
            // manually, just in case
            // ConsoleLog.Message(Environment.NewLine + "Run this command in enlistment to start manually merge:");
            // ConsoleLog.Warning(Commands.DiffAll());
            ConsoleLog.Message(Environment.NewLine + "Run this command in enlistment to start merge check:");
            ConsoleLog.Warning(Commands.DiffChangesOnly());
        }
    }
}

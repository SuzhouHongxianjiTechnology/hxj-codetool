namespace MergeTool
{
    using System;
    using MergeTool.Execution;
    using Mint.Common;

    class Program
    {
        public static void Main(string[] args)
        {
            bool hasArgs = args != null && args.Length > 0;
            string userInput = hasArgs ? args[0] : string.Empty;

            try
            {
                switch (userInput)
                {
                    case "auto":
                        Merger.AutoMerge();
                        break;
                    case "update":
                        Updater.UpdateSubstratePackagesProps();
                        break;
                    case "nuspec":
                        Updater.UpdateSubstrateNugetPackages();
                        break;
                    case "sort":
                        Updater.SortSubstrateRestoreEntry();
                        break;
                    case "cso":
                        Consumer.ConsumeCso();
                        break;
                    case "pop3":
                        Consumer.ConsumePop3();
                        break;
                    case "set":
                        OtherActions.OpenSettings();
                        break;
                    case "cmd":
                        OtherActions.ShowCommands();
                        break;
                    default:
                        ShowUsage();
                        break;
                }
                Console.ResetColor();
            }
            catch (Exception e)
            {
                ConsoleLog.Error(Environment.NewLine + $"{e}");
                return;
            }
        }

        private static void ShowUsage()
        {
            ConsoleLog.Title(Environment.NewLine + "Merge tool usage:" + Environment.NewLine);
            ConsoleLog.Warning("  > mg auto    - Auto merge.");
            ConsoleLog.Warning("  > mg update  - Update packages.");
            ConsoleLog.Warning("  > mg nuspec  - Update nuspec files.");
            ConsoleLog.Warning("  > mg sort    - Sort entry points.");
            ConsoleLog.Warning("  > mg cso     - Consume CSO.");
            ConsoleLog.Warning("  > mg pop3    - Consume POP3.");
            ConsoleLog.Warning("  > mg set     - Open settings file.");
            ConsoleLog.Warning("  > mg cmd     - Show git difftool commands.");
        }
    }
}

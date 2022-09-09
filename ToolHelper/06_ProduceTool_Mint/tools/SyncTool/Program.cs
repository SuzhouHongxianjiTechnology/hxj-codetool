namespace SyncTool
{
    using System;
    using Mint.Common.Utilities;

    class Program
    {
        public static void Main(string[] args)
        {
            UserInputParser.Parse(args, out string command, out _);

            try
            {
                switch (command)
                {
                    case "auto":
                        Actions.AutoSync();
                        break;
                    case "update":
                        Actions.UpdatePackagesProps();
                        break;
                    case "nuspec":
                        Actions.UpdateNuGetPackages();
                        break;
                    case "cso":
                        Actions.ConsumeCso();
                        break;
                    case "pop3":
                        Actions.ConsumePop3();
                        break;
                    case "set":
                        Actions.OpenSettings();
                        break;
                    case "cmd":
                        Actions.ShowCommands();
                        break;
                    default:
                        ShowUsage();
                        break;
                }
                Console.ResetColor();
            }
            catch (Exception e)
            {
                ConsoleLog.Error($"\n{e}");
                return;
            }
        }

        private static void ShowUsage()
        {
            ConsoleLog.Title("Merge tool usage:\n");
            ConsoleLog.Warning("  > mg auto    - Auto merge.");
            ConsoleLog.Warning("  > mg update  - Update packages.");
            ConsoleLog.Warning("  > mg nuspec  - Update nuspec files.");
            ConsoleLog.Warning("  > mg cso     - Consume CSO.");
            ConsoleLog.Warning("  > mg pop3    - Consume POP3.");
            ConsoleLog.Warning("  > mg set     - Open settings file.");
            ConsoleLog.Warning("  > mg cmd     - Show git difftool commands.");
        }
    }
}

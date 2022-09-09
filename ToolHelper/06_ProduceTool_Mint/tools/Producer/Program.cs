namespace Producer
{
    using System;
    using Mint.Common.Utilities;
    using Mint.Substrate.Construction;

    class Program
    {
        static void Main(string[] args)
        {
            UserInputParser.Parse(args, out string command, out string keyword, out string _);

            try
            {
                switch (command)
                {
                    case "netstd":
                        Actions.Produce(Frameworks.NetStd); break;
                    case "netcore":
                        Actions.Produce(Frameworks.NetCore); break;
                    case "move":
                        if (string.IsNullOrEmpty(keyword))
                            ConsoleLog.Error("Please specify a component name for moving.");
                        else
                            Actions.Move(keyword); break;
                    case "tp":
                        MoveManager.ReplaceTargetPath();
                        break;
                    case "any":
                        if (string.IsNullOrEmpty(keyword))
                            ConsoleLog.Error("Please specify a keyword to search for.");
                        else
                            Actions.FindAnyString(keyword); break;
                    case "proj":
                        if (string.IsNullOrEmpty(keyword))
                            ConsoleLog.Error("Please specify a project name to search for.");
                        else
                            Actions.GetProducedInfo(keyword); break;
                    case "pkg":
                        if (string.IsNullOrEmpty(keyword))
                            ConsoleLog.Error("Please specify a package name to search for");
                        else
                            Actions.GetPackageVersion(keyword); break;
                    case "update":
                        Actions.UpdateSubstratePackages(); break;
                    default:
                        ShowUsage(); break;
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
            ConsoleLog.Title("Produce tool usage:\n");
            ConsoleLog.Warning("  > produce netstd  - Target framework 'netstandard2.0'");
            ConsoleLog.Warning("  > produce netcore - Target framework 'netcoreapp3.1'");
            ConsoleLog.Warning("  > produce move [dirs] - Move projects to dev root.");
            ConsoleLog.Warning("  > produce tp      - Update all target paths.");
            ConsoleLog.Warning(" ");
            ConsoleLog.Warning("  > produce any  [any  keyword] - Find any keyword in produced projects.");
            ConsoleLog.Warning("  > produce proj [project name] - Get project info by project name.");
            ConsoleLog.Warning("  > produce pkg  [package name] - Get package version by package name.");
            ConsoleLog.Warning(" ");
        }
    }
}

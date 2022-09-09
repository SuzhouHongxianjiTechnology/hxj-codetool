namespace ProduceTool
{
    using System;
    using Mint.Common;
    using Mint.Substrate.Construction;
    using ProduceTool.Execution;

    class Program
    {
        static void Main(string[] args)
        {
            bool hasArgs = args != null && args.Length > 0;
            string userInput = hasArgs ? args[0] : string.Empty;

            try
            {
                switch (userInput.ToLower())
                {
                    case "netstd":
                        Producer.Produce(TargetFramework.NetStd); break;
                    case "netcore":
                        Producer.Produce(TargetFramework.NetCore); break;
                    case "any":
                        Searcher.SearchString(args); break;
                    case "1701":
                        Searcher.SearchNoWarn("NU1701"); break;
                    default:
                        ShowUsage(); break;
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
            ConsoleLog.Title(Environment.NewLine + "Produce tool usage:" + Environment.NewLine);
            ConsoleLog.Warning("  > produce netstd        - Produce .NetStandard project.");
            ConsoleLog.Warning("  > produce netcore       - Produce .NetCore project.");
            ConsoleLog.Warning("  > produce any [keyword] - Search keyword in all projects.");
            ConsoleLog.Warning("  > produce 1701          - Find all NU1701 packages.");
            ConsoleLog.Warning(" ");
        }
    }
}

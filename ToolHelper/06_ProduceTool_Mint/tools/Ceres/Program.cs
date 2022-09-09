namespace Ceres
{
    using System;
    using Mint.Common.Utilities;

    class Program
    {
        static void Main(string[] args)
        {
            UserInputParser.Parse(args, out string command, out string arg, out string _);

            try
            {
                switch (command)
                {
                    case "plan":
                        Actions.GenerateMovePlan(arg); break;
                    case "move":
                        Actions.MoveWave(arg); break;
                    case "dep":
                        Actions.PrintRelationship(arg); break;
                    case "types":
                        Actions.ProcessCeresTypes(arg); break;
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
            ConsoleLog.Title("Ceres tool usage:\n");
            ConsoleLog.Warning("  > ceres plan - Generate Ceres Move-Plan");
            ConsoleLog.Warning("  > ceres move - working on it...");
            ConsoleLog.Warning("  > ceres dep  - Display Ceres Relationship");
        }
    }
}

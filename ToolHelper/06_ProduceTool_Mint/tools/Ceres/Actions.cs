namespace Ceres
{
    using System;
    using System.IO;
    using System.Linq;
    using Mint.Common.Utilities;
    using Mint.Database.Configurations;

    internal static class Actions
    {
        private const string CeresPrefix = "Microsoft.Ceres";

        internal static void GenerateMovePlan(string process)
        {
            using (Timer.TimeThis)
            {
                var allTypeFile = DatabaseSettings.Settings.AllTypesJson;
                if (!File.Exists(allTypeFile))
                {
                    ConsoleLog.Error($"File cannot be found: '{allTypeFile}'.");
                    return;
                }
                var fileName = Path.GetFileNameWithoutExtension(allTypeFile);
                string version = fileName.Split('_').First();

                ConsoleLog.Title($"\nGenerating Ceres Move-Plan for {process}.");
                var movePlaner = new MovePlaner(version);
                ConsoleLog.Warning($"Version: {movePlaner.Version}");

                ConsoleLog.InLine("\n--", ConsoleColor.DarkGray);
                ConsoleLog.InLine($" Build Graph ", ConsoleColor.White);
                ConsoleLog.Ignore("---------------------------------------------------");
                movePlaner.BuildGraph(process);

                ConsoleLog.InLine("\n--", ConsoleColor.DarkGray);
                ConsoleLog.InLine($" Generate Move Plan ", ConsoleColor.White);
                ConsoleLog.Ignore("--------------------------------------------");
                var waveItems = movePlaner.GenerateMovePlan();

                ConsoleLog.InLine("\n--", ConsoleColor.DarkGray);
                ConsoleLog.Ignore("----------------------------------------------------------------");
                movePlaner.SaveToJson(waveItems);
                ConsoleLog.InLine("The plan has been saved to: ");
                ConsoleLog.Success($"{Files.move_plan}.json");
            }
        }

        internal static void MoveWave(string waveNumber)
        {
            if (Int32.TryParse(waveNumber, out int number))
            {
                var waves = MoveExecutor.LoadPlan();
                int totalWaves = waves.Count();
                if (0 < number && number <= totalWaves)
                {
                    MoveExecutor.MoveWave(waves[number - 1]);
                    return;
                }
            }
            throw new ArgumentException("Invalid wave number.");
        }

        internal static void PrintRelationship(string process)
        {
            var allTypeFile = DatabaseSettings.Settings.AllTypesJson;
            if (!File.Exists(allTypeFile))
            {
                ConsoleLog.Error($"File cannot be found: '{allTypeFile}'.");
                return;
            }
            var fileName = Path.GetFileNameWithoutExtension(allTypeFile);
            string version = fileName.Split('_').First();

            ConsoleLog.Title("\nCalculating All Moved Ceres Relationship.");
            var movePlan = new MovePlaner(version);
            ConsoleLog.Warning($"Version: {movePlan.Version}");

            ConsoleLog.InLine("\n--", ConsoleColor.DarkGray);
            ConsoleLog.InLine($" Build Graph ", ConsoleColor.White);
            ConsoleLog.Ignore("---------------------------------------------------");
            movePlan.BuildGraph(process, ignoreMoved: true);

            ConsoleLog.InLine("\n--", ConsoleColor.DarkGray);
            ConsoleLog.InLine($" Calculate Relationship ", ConsoleColor.White);
            ConsoleLog.Ignore("--------------------------------------------");
            movePlan.DisplayRelationship();
        }

        internal static void ProcessCeresTypes(string process)
        {
            using (Timer.TimeThis)
            {
                var allTypeFile = DatabaseSettings.Settings.AllTypesJson;
                if (!File.Exists(allTypeFile))
                {
                    ConsoleLog.Error($"File cannot be found: '{allTypeFile}'.");
                    return;
                }
                var fileName = Path.GetFileNameWithoutExtension(allTypeFile);
                string version = fileName.Split('_').First();

                ConsoleLog.Title($"\n{process} used Ceres types:");
                var movePlaner = new MovePlaner(version);
                ConsoleLog.Warning($"Version: {movePlaner.Version}");

                movePlaner.ProcessUsedCeresTypes(process);
            }
        }
    }
}

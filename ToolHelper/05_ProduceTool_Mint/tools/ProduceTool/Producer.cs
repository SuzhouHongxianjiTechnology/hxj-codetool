namespace ProduceTool.Execution
{
    using System;
    using System.IO;
    using Mint.Common;
    using Mint.Substrate.Production;

    internal static class Producer
    {
        internal static void Produce(string framework)
        {
            ValidationUtils.VerifyThrowValidNetFrameworkFolder();
            string project = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.csproj")[0];

            ConsoleLog.Title("Producing project:");
            ConsoleLog.Path($"{project}");
            ConsoleLog.Ignore("----------------------------------------------------------------");

            Timer.Start();

            var srcDir = Environment.GetEnvironmentVariable("SRCDIR");
            PortingConfig config = PortingConfig.Create(project, framework);
            ProduceManager manager = new ProduceManager(srcDir, config);

            ConsoleLog.LogAction("Reseting restore entry ....", () => manager.ResetRestoreEntry());

            ConsoleLog.LogAction("Reseting dirs file ........", () => manager.ResetDirsProj());

            ConsoleLog.LogAction("Producing project .........", () => manager.ProduceProject());

            ConsoleLog.LogAction("Producing restore entry ...", () => manager.ProduceRestoreEntry());

            ConsoleLog.LogAction("Producing dirs file .......", () => manager.ProduceDirsProj());

            ConsoleLog.LogAction("Final touch ...............", () => manager.FormatProjectFile());

            manager.AnalyizeProject();

            Timer.Stop();
        }
    }
}

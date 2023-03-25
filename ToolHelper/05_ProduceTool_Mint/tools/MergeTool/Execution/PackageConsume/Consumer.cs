namespace MergeTool.Execution
{
    using MergeTool.Common;
    using Mint.Common;

    internal static class Consumer
    {
        internal static void ConsumeCso()
        {
            Git csoGit = new Git(Settings.CSOSrc);
            csoGit.ResetBranch("master");

            ConsoleLog.Title("Consuming CSO ...");
            ConsoleLog.Ignore("----------------------------------------------------------------");

            ConsoleLog.Message("Updating CSO client csproj ...", inLine: true);
            CsoConsumer.UpdateCsoClientCsproj();
            ConsoleLog.Success(" [DONE]");

            ConsoleLog.Message("Updating CSO client nuspec ...", inLine: true);
            CsoConsumer.UpdateCsoClientNuspec();
            ConsoleLog.Success(" [DONE]");

            ConsoleLog.Message("Updating CSO packages ...", inLine: true);
            CsoConsumer.UpdateCsoPackagesProps();
            ConsoleLog.Success(" [DONE]");
        }

        internal static void ConsumePop3()
        {
            Git popGit = new Git(Settings.Pop3Src);
            popGit.ResetBranch("master");

            ConsoleLog.Title("Consuming POP3 ...");
            ConsoleLog.Ignore("----------------------------------------------------------------");

            ConsoleLog.Message("Updating POP3 ModelB2 ...", inLine: true);
            Pop3Consumer.UpdatePop3ModelB2();
            ConsoleLog.Success(" [DONE]");
        }
    }
}

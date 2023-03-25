namespace MergeTool.Execution
{
    using System.IO;
    using MergeTool.Common;
    using Mint.Substrate.Constants;
    using Mint.Substrate.Construction;
    using Mint.Common;
    using Mint.Substrate.Utilities;

    internal static class Pop3Consumer
    {
        private static readonly string Pop3ModelB2 = Path.Combine(Settings.Pop3Src, "Microsoft.Exchange.Pop3.ModelB2");

        private static readonly string CsoClient = Path.Combine(Settings.Pop3Src, "Microsoft.M365.CoreStoreObjects.Client");

        internal static void UpdatePop3ModelB2()
        {
            using (var pop3service = ConstFiles.Pop3Service)
            {
                string csoVersion = ConstFiles.CsoClientCsproj.GetProperty(Tags.FileVersion)?.Value;
                foreach (var package in pop3service.PackageReferences)
                {
                    if (StringUtils.EqualsIgnoreCase(package.Name, Pop3ModelB2))
                    {
                        package.Version = string.Format("[{0}]", Settings.PackageVersion);
                    }
                    else if (StringUtils.EqualsIgnoreCase(package.Name, CsoClient))
                    {
                        package.Version = string.Format("[{0}]", csoVersion);
                    }
                }
            }
        }
    }
}

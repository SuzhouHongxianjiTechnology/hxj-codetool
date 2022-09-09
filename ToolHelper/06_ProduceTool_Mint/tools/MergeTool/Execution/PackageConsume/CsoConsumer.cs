namespace MergeTool.Execution
{
    using System;
    using System.Collections.Generic;
    using MergeTool.Common;
    using Mint.Substrate.Constants;
    using Mint.Substrate.Construction;
    using Mint.Common;

    internal static class CsoConsumer
    {
        private static List<string> CsoConsumeNugets = new List<string>
        {
            "Microsoft.Exchange.MapiAbstraction",
            "Microsoft.Exchange.MapiImplementation"
        };

        internal static void UpdateCsoClientCsproj()
        {
            using (var csoCsproj = ConstFiles.CsoClientCsproj)
            {
                var versionElement = csoCsproj.GetProperty(Tags.FileVersion);
                if (versionElement != null)
                {
                    Version version = new Version(versionElement.Value);
                    versionElement.SetValue(version.IncrementRevision());
                }
            }
        }

        internal static void UpdateCsoClientNuspec()
        {
            using (var csoNuspec = ConstFiles.CsoClientNuspec)
            {
                Version version = new Version(csoNuspec.Version);
                csoNuspec.Version = version.IncrementRevision().ToString();

                string nugetVersion = string.Format("[{0}]", Settings.PackageVersion);
                foreach (var package in CsoConsumeNugets)
                {
                    csoNuspec.SetDependencyVersion(package, nugetVersion);
                }
            }
        }

        internal static void UpdateCsoPackagesProps()
        {
            using (var csoProps = ConstFiles.CsoPackagesProps)
            {
                string version = string.Format("[{0}]", Settings.PackageVersion);
                foreach (var package in CsoConsumeNugets)
                {
                    csoProps.SetPackageVersion(package, version);
                }
            }
        }
    }
}

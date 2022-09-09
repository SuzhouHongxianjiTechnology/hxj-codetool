namespace Mint.Substrate.Constants
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public static class SpecialCases
    {
        public static readonly List<string> KnownCppProjects = new List<string> {
            "AsyncDnsManaged2",
            "Interop.NetFw",
            "Microsoft.Exchange.Compliance.CrimsonEvents",
            "Microsoft.Exchange.DsApi.ClientSlimRpc",
            "Microsoft.Exchange.Rpc.KVCache",
            "Microsoft.Exchange.Rpc",
            "Microsoft.Exchange.StructuredQuery"
        };

        public static readonly List<string> KnownNetCoreOnlyFiles = new List<string>
        {
            Path.Combine(DF.SrcDir, @"sources\dev\Forge\src\Platform.Store.InMemory.NetCore\Platform.Store.InMemory.NetCore.csproj"),
            Path.Combine(DF.SrcDir, @"sources\dev\NetFxToNetStdNetCore\src\SystemWebAdapters.NetCore\Microsoft.Exchange.SystemWebAdapters.NetCore.csproj"),
            Path.Combine(DF.SrcDir, @"sources\test\NetFxToNetStdNetCore\src\SystemWebAdapters.UnitTests.NetCore\Internal.Exchange.SystemWebAdapters.UnitTests.NetCore.csproj"),
            Path.Combine(DF.SrcDir, @"sources\dev\Azure\nupkg\Microsoft.Exchange.Azure.SecretsProvider.Standard\Microsoft.Exchange.Azure.SecretsProvider.Standard.csproj"),
            Path.Combine(DF.SrcDir, @"sources\dev\cafe\nupkg\Microsoft.Exchange.Routing.EndpointLookup.Core\Microsoft.Exchange.Routing.EndpointLookup.Core.csproj"),
            Path.Combine(DF.SrcDir, @"sources\dev\common\nupkg\Diagnostics\Microsoft.Exchange.Diagnostics.Core\Microsoft.Exchange.Diagnostics.Core.csproj"),
            Path.Combine(DF.SrcDir, @"sources\dev\PopImap\nupkg\Imap4.ModelA\Microsoft.Exchange.Imap4.ModelA.csproj"),
            Path.Combine(DF.SrcDir, @"sources\dev\PopImap\nupkg\InMemoryMapiImplementation\Microsoft.Exchange.InMemoryMapiImplementation.csproj"),
            Path.Combine(DF.SrcDir, @"sources\dev\PopImap\nupkg\MapiAbstraction\Microsoft.Exchange.MapiAbstraction.csproj"),
            Path.Combine(DF.SrcDir, @"sources\dev\PopImap\nupkg\MapiImplementation\Microsoft.Exchange.MapiImplementation.csproj"),
            Path.Combine(DF.SrcDir, @"sources\dev\PopImap\nupkg\Pop3.ModelB2\Microsoft.Exchange.Pop3.ModelB2.csproj"),
            Path.Combine(DF.SrcDir, @"sources\dev\PopImap\nupkg\Pop3\Microsoft.Exchange.Pop3.Nuget.csproj"),
            Path.Combine(DF.SrcDir, @"sources\dev\PopImap\nupkg\XSOCommon\Microsoft.Exchange.XSOCommon.csproj"),
            Path.Combine(DF.SrcDir, @"sources\dev\PopImap\nupkg\XSO\Microsoft.Exchange.XSO.csproj"),
        };

        public static readonly List<string> KnownNetFrameworkOnlyFiles = new List<string>
        {
        };

        public static readonly Dictionary<string, string> PackageReplacement = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"Microsoft.Cloud.InstrumentationFramework",         "Microsoft.Cloud.InstrumentationFramework.NetStd"},
            {"Microsoft.Cloud.InstrumentationFramework.Events",  "Microsoft.Cloud.InstrumentationFramework.NetStd"},
            {"Microsoft.Cloud.InstrumentationFramework.Metrics", "Microsoft.Cloud.InstrumentationFramework.NetStd"},
            {"Microsoft.Office.ActiveMonitoring.Local.Runtime",  "Microsoft.Office.Datacenter.WorkerTaskFrameworkInternalProvider.Pop3"},
            {"Microsoft.Office.Datacenter.PassiveMonitoring",    "Microsoft.M365.Core.PassiveMonitoring"},
            {"Parallax.Core",                                    null},
            {"RPSv",                                             "RPSv6.7"},
            {"TorusSDK",                                         "Torus.Std"},
        };

        public static readonly Dictionary<string, (string, string)> AdditinalProjects = new Dictionary<string, (string, string)>(StringComparer.OrdinalIgnoreCase)
        {
            {"Microsoft.Exchange.Rpc", (Path.Combine(DF.SrcDir, @"sources\dev\common\src\Rpc\dll.NetCore\Microsoft.Exchange.Rpc.NetCore.vcxproj"),
                                        @"$(TargetPathDir)dev\common\Microsoft.Exchange.Rpc.NetCore\$(FlavorPlatformDir)\Microsoft.Exchange.Rpc.dll")},
        };
    }
}

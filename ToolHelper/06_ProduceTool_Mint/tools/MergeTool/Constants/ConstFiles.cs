namespace Mint.Substrate.Constants
{
    using System.IO;
    using MergeTool.Common;
    using Mint.Substrate.Construction;

    public static class ConstFiles
    {
        public static readonly NetCoreProjectFile CsoClientCsproj = new NetCoreProjectFile(Path.Combine(Settings.CSOSrc, @"sources/CoreStoreObjects/Dev/Client/Microsoft.M365.CoreStoreObjects.Client.csproj"));

        public static readonly NuspecProjectFile CsoClientNuspec = new NuspecProjectFile(Path.Combine(Settings.CSOSrc, @"sources/CoreStoreObjects/NugetPackging/Microsoft.M365.CoreStoreObjects.Client.nuspec"));

        public static readonly PackagesPropsFile CsoPackagesProps = new PackagesPropsFile(Path.Combine(Settings.CSOSrc, @"Packages.props"));

        public static readonly NetCoreProjectFile Pop3Service = new NetCoreProjectFile(Path.Combine(Settings.Pop3Src, @"sources/dev/Pop3Service/Microsoft.M365.Pop3.Service.csproj"));

        public static readonly string HardCodeH = Path.Combine(Settings.DFSrc, @"sources/dev/store/src/exrpc32/exrpc/hardcodedbldver.h");
    }
}

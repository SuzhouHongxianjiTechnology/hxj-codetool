namespace Mint.Substrate
{
    using System;
    using System.IO;
    using Mint.Substrate.Constants;
    using Mint.Substrate.Construction;
    using Mint.Substrate.Utilities;

    public static class DF
    {
        public static string SrcDir
        {
            get
            {
                Verification.VerifyThrowEnvironmentVariableNotDefined("SRCDIR");
                var srcDir = Environment.GetEnvironmentVariable("SRCDIR");
                Verification.VerifyThrowDirectoryNotExists(srcDir);
                return srcDir;
            }
        }

        public static readonly VariantConfigurationFile VariantConfig = new VariantConfigurationFile(
            Path.Combine(DF.SrcDir, ConstPaths.VariantConfiguration)
        );

        public static readonly CorextConfigFile InnerCorext = new CorextConfigFile(
            Path.Combine(DF.SrcDir, ConstPaths.InnerCorextConfig)
        );

        public static readonly CorextConfigFile OuterCorext = new CorextConfigFile(
            Path.Combine(DF.SrcDir, ConstPaths.OuterCorextConfig)
        );

        public static readonly PackagesPropsFile PackagesProps = new PackagesPropsFile(
            Path.Combine(DF.SrcDir, ConstPaths.PackagesProps)
        );

        public static readonly RestoreEntryFile RestoreEntry = new RestoreEntryFile(
            Path.Combine(DF.SrcDir, ConstPaths.RestoreEntry)
        );
    }
}

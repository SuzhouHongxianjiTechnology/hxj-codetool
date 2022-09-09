namespace Mint.Substrate
{
    using System;
    using System.IO;
    using Mint.Substrate.Configurations;
    using Mint.Substrate.Construction;

    public static class Repo
    {
        public static SubstratePaths Paths = SubstratePaths.Settings;

        public static readonly RestoreFile RestoreEntry = Load<RestoreFile>(
            Path.Combine(Repo.Paths.SrcDir, Repo.Paths.RestoreEntry)
        );

        public static readonly PropsFile PackagesProps = Load<PropsFile>(
            Path.Combine(Repo.Paths.SrcDir, Repo.Paths.PackagesProps)
        );

        public static readonly CorextFile InnerCorext = Load<CorextFile>(
            Path.Combine(Repo.Paths.SrcDir, Repo.Paths.InnerCorext)
        );

        public static readonly CorextFile OuterCorext = Load<CorextFile>(
            Path.Combine(Repo.Paths.SrcDir, Repo.Paths.OuterCorext)
        );

        public static readonly TargetsFile VarConfigV1 = Load<TargetsFile>(
            Path.Combine(Repo.Paths.SrcDir, Repo.Paths.VariantConfigV1)
        );

        public static readonly TargetsFile VarConfigV2 = Load<TargetsFile>(
            Path.Combine(Repo.Paths.SrcDir, Repo.Paths.VariantConfigV2)
        );

        // public static readonly string HardCodedBV = Path.Combine(SrcDir, Settings.Read("HARD_CODED_BLDVER", "Paths"));

        public static XMLFile Load(string path)
        {
            return new XMLFile(path);
        }

        public static T Load<T>(string path) where T : XMLFile
        {
            T? instance = Activator.CreateInstance(typeof(T), new object[] { path }) as T;

            if (instance == null)
            {
                throw new Exception($"Failed to parse file as {typeof(T)}. (File {path})");
            }

            return instance;
        }

        public static TextDirsFile LoadTextDirsFile(string path)
        {
            return new TextDirsFile(path);
        }
    }
}

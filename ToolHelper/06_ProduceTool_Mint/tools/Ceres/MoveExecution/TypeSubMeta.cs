namespace Ceres
{
    using System.IO;

    public class TypeSubMeta
    {
        public string FastNamespace { get; }
        public string SubAssemblyName { get; }
        public string SubRootFolder { get; }
        public string SubSourceFolder { get; }
        public string SubPorjectFile { get; }

        public TypeSubMeta(string subSrcDir, string fastFullNamespace)
        {
            this.FastNamespace = fastFullNamespace;

            var parts = this.FastNamespace.Split(',');
            var name = parts[0].Split('.');
            this.SubAssemblyName = string.Join('.', name[0], "Exchange", name[1], name[2]);

            var folders = parts[1].Split('.')[3..];
            this.SubRootFolder = Path.Combine(subSrcDir, @"sources\dev\common\src\Ceres", SubAssemblyName);

            string subFolder = string.Empty;
            foreach (var folder in folders)
            {
                subFolder = Path.Combine(subFolder, folder);
            }
            this.SubSourceFolder = Path.Combine(this.SubRootFolder, subFolder);

            this.SubPorjectFile = Path.Combine(this.SubRootFolder, this.SubAssemblyName + ".csproj");
        }
    }
}

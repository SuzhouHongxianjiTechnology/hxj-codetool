namespace Mint.Substrate.Construction
{
    using Mint.Common.Extensions;

    public class CorextFile : XMLFile
    {
        public CorextFile(string path) : base(path) { }

        public PackageSet GetPackages()
        {
            var packages = this.Document.GetFirst(Tags.Packages).GetAll(Tags.Package);
            return new PackageSet(packages, Tags.id, Tags.version);
        }
    }
}

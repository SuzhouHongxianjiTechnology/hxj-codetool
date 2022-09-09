namespace Mint.Substrate.Construction
{
    using Mint.Common.Extensions;

    public class PropsFile : XMLFile
    {
        public PropsFile(string path) : base(path) { }

        public PackageSet GetPackages()
        {
            var packages = this.Document.GetAll(Tags.PackageReference);
            return new PackageSet(packages, Tags.Update, Tags.Version);
        }
    }
}

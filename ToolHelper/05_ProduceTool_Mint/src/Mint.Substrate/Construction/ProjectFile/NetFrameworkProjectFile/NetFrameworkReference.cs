namespace Mint.Substrate.Construction
{
    using System.Linq;
    using System.Xml.Linq;
    using Mint.Common;
    using Mint.Substrate.Constants;
    using Mint.Substrate.Utilities;

    public class NetFrameworkReference : ProjectElement
    {
        private const string PackagePrefix = "$(Pkg";

        private string Include { get; }

        private string HintPath { get; }

        public ReferenceType Type { get; }

        public string Name { get; }

        public NetFrameworkReference(XElement element) : base(element)
        {
            // this.Include
            var include = this.Element.GetAttribute(Tags.Include)?.Value.Replace(".dll", "");
            include = include.EndsWith(".dll") ? include.Substring(0, include.Length - 4) : include;
            include = include.EndsWith(".exe") ? include.Substring(0, include.Length - 4) : include;
            this.Include = include;

            // this.HintPath
            this.HintPath = this.Element.GetFirst(Tags.HintPath)?.Value;

            bool isIncludePackage = this.Include?.StartsWith(PackagePrefix) == true;
            bool isHintPathPackage = this.HintPath?.StartsWith(PackagePrefix) == true;
            bool isPackage = isIncludePackage || isHintPathPackage;

            // this.Name
            if (isPackage)
            {
                string text = this.HintPath ?? this.Include;
                this.Name = SubstrateUtils.ResolvePackageName(text);
            }
            else
            {
                this.Name = this.Include.Split("\\").Last().Replace(".dll", "");
            }

            // this.Source
            if (NetFrameworkConsts.KnownSDKS.Contains(this.Name))
            {
                this.Type = ReferenceType.SDK;
            }
            else if (isPackage || NetFrameworkConsts.KnownNugets.Contains(this.Name))
            {
                this.Type = ReferenceType.Nuget;
            }
            else
            {
                this.Type = ReferenceType.Substrate;
            }
        }
    }
}

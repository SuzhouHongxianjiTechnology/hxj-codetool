namespace Mint.Substrate.Construction
{
    using System;
    using System.Xml.Linq;
    using Mint.Common;
    using Mint.Substrate.Constants;

    public class NetCorePackageReference : ProjectElement
    {
        public string Name { get; }

        public string Version
        {
            get => this.Element.GetAttribute(Tags.Version)?.Value;
            set
            {
                if (this.Element.HasAttribute(Tags.Version))
                {
                    this.Element.SetAttributeValue(Tags.Version, value);
                }
            }
        }

        public bool IsCompatible { get; }

        public bool IsUndefined => this.Element.HasAttribute(Tags.Undefined);

        public NetCorePackageReference(XElement element) : base(element)
        {
            this.Name = this.Element.GetAttribute(Tags.Include)?.Value;
            this.IsCompatible = NetCoreConsts.NugetCompatibility.ContainsKey(this.Name) &&
                                NetCoreConsts.NugetCompatibility[this.Name];
        }

        public bool HasNoWarn(string warningCode)
        {
            string attrNowarn = this.Element.GetAttribute(Tags.NoWarn)?.Value;
            string chidNowarn = this.Element.GetFirst(Tags.NoWarn)?.Value;
            return string.Equals(warningCode, attrNowarn) || string.Equals(warningCode, chidNowarn);
        }

        public override bool Equals(object obj)
        {
            return obj is NetCorePackageReference reference &&
                   Name == reference.Name &&
                   Version == reference.Version;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Version);
        }
    }
}

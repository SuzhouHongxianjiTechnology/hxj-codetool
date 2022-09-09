namespace Mint.Substrate.Construction
{
    using System;
    using System.Xml.Linq;
    using Mint.Common.Extensions;

    public class Package
    {
        private XElement element;
        private string nameTag;
        private string versionTag;

        public string Name
        {
            get => this.element.GetAttribute(this.nameTag).Value;
        }

        public string Version
        {
            get => this.element.GetAttribute(this.versionTag).Value;
            set => this.element.SetAttributeValue(this.versionTag, value);
        }

        public Package(XElement element, string nameTag, string versionTag)
        {
            this.element = element;
            this.nameTag = nameTag;
            this.versionTag = versionTag;
        }

        public bool TryUpgrade(string newVersion)
        {
            try
            {
                var curVer = new Version(Version);
                var newVer = new Version(newVersion);
                if (curVer.CompareTo(newVer) < 0)
                {
                    this.Version = newVersion;
                }
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}

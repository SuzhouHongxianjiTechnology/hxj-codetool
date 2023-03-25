namespace Mint.Substrate.Construction
{
    using System.Xml.Linq;
    using Mint.Common;

    public class NetCoreReference : ProjectElement
    {
        public string Name
        {
            get => this.Element.GetAttribute(Tags.Include)?.Value;
            set => this.Element.SetAttributeValue(Tags.Include, value);
        }

        public string HintPath
        {
            get => this.Element.GetFirst(Tags.HintPath)?.Value;
            set => this.Element.SetAttributeValue(Tags.Include, value);
        }

        public bool IsBlocked
        {
            get => this.Element.HasAttribute(Tags.Blocked);
        }

        public NetCoreReference(XElement element) : base(element) { }

        public bool HasNoWarn(string warningCode)
        {
            string attrNowarn = this.Element.GetAttribute(Tags.NoWarn)?.Value;
            string chidNowarn = this.Element.GetFirst(Tags.NoWarn)?.Value;
            return string.Equals(warningCode, attrNowarn) || string.Equals(warningCode, chidNowarn);
        }
    }
}

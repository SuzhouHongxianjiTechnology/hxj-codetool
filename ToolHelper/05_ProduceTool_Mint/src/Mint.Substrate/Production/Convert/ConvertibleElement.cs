namespace Mint.Substrate.Production
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;
    using Mint.Common;
    using Mint.Substrate.Construction;

    public sealed class ConvertibleElement : ProjectElement
    {
        private PortingConfig _config;

        private ElementConverter _converter;

        private string _convertSignature;

        public ConvertResult ConvertResult { get; }

        public List<XElement> ProducedElements { get; }

        public XElement LastSibling => this.Element.LastSibling();

        public static ConvertibleElement Parse(XElement element, PortingConfig config)
        {
            return new ConvertibleElement(element, config);
        }

        private ConvertibleElement(XElement element, PortingConfig config) : base(element)
        {
            this._config = config;

            this._converter = ElementConverters.GetConverter(element.Name.LocalName);

            {
                string condition = this.Element.GetAttribute(Tags.Condition)?.Value;
                if (StringUtils.EqualsIgnoreCase(condition, "$(DefineConstants.Contains('NETFRAMEWORK'))"))
                {
                    this.ConvertResult = ConvertResult.Removed;
                    this.ProducedElements = new List<XElement>();
                    this._convertSignature = string.Empty;
                }
                else
                {
                    if (StringUtils.EqualsIgnoreCase(condition, "!$(DefineConstants.Contains('NETFRAMEWORK'))"))
                    {
                        this.Element.GetAttribute(Tags.Condition)?.Remove();
                    }
                    this.ConvertResult = this.ProduceClone(out List<XElement> elements);
                    this.ProducedElements = elements;
                    foreach (var e in elements)
                    {
                        this._convertSignature += e.ToString();
                    }
                }
            }
        }

        public ConvertResult ProduceClone(out List<XElement> elements)
        {
            return this._converter.Invoke(this.Element, this._config, out elements);
        }

        public void Produce()
        {
            switch (this.ConvertResult)
            {
                case ConvertResult.Replaced:
                    this.Element.ReplaceWith(this.ProducedElements);
                    break;
                case ConvertResult.Removed:
                    this.Element.TryRemove();
                    break;
                case ConvertResult.NotChanged:
                default:
                    break;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is ConvertibleElement other &&
                   this.ConvertResult == other.ConvertResult &&
                   string.Equals(this._convertSignature, other._convertSignature);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.ConvertResult, this._convertSignature);
        }
    }
}

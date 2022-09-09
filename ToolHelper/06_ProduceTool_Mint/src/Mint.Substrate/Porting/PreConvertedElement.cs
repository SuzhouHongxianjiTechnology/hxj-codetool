namespace Mint.Substrate.Porting
{
    using System.Xml.Linq;
    using Mint.Common.Extensions;

    public class PreConvertedElement
    {
        private ConvertResult result;

        private XElement source;

        private XElement? element;

        private bool doneConvert;

        public ConvertResult ConvertResult => this.result;

        public XElement? Element => this.element;

        public PreConvertedElement(XElement source, ConvertResult result, XElement? element)
        {
            this.source = source;
            this.result = result;
            this.element = element;
            this.doneConvert = false;
        }

        public void Commit()
        {
            if (this.doneConvert) return;

            switch (this.result)
            {
                case ConvertResult.Removed:
                    this.source.TryRemove();
                    break;

                case ConvertResult.Changed:
                    this.source.ReplaceWith(this.element);
                    break;

                case ConvertResult.NotChanged:
                    break;
            }
            this.doneConvert = true;
        }
    }
}

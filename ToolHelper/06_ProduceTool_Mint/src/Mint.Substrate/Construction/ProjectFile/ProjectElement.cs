namespace Mint.Substrate.Construction
{
    using System.Xml.Linq;
    using Mint.Common;

    public abstract class ProjectElement
    {
        internal protected XElement Element { get; }

        protected ProjectElement(XElement element)
        {
            this.Element = element;
        }

        public bool TryRemove()
        {
            return this.Element.TryRemove();
        }
    }
}

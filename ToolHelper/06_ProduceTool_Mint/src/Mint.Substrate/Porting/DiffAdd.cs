namespace Mint.Substrate.Porting
{
    using System.Xml.Linq;
    using Mint.Substrate.Construction;

    public class DiffAdd : IDiff
    {
        private PortableElement pe;

        public DiffAdd(PortableElement pe)
        {
            this.pe = pe;
        }

        public SyncResult SyncTo(BuildFile file)
        {
            var curr = this.pe.Current;
            var last = this.pe.Last;
            var next = this.pe.Next;

            XElement? actualElement;

            if (file.TryGetElement(last?.Element, out actualElement))
            {
                actualElement.AddAfterSelf(curr.Element);
                return SyncResult.Succeed;
            }

            if (file.TryGetElement(next?.Element, out actualElement))
            {
                actualElement.AddBeforeSelf(curr.Element);
                return SyncResult.Succeed;
            }

            if (file.TryAddElement(curr.Element))
            {
                return SyncResult.Succeed;
            }

            return SyncResult.Failed;
        }
    }
}

namespace Mint.Substrate.Porting
{
    using System.Xml.Linq;
    using Mint.Common.Extensions;
    using Mint.Substrate.Construction;

    public class DiffDel : IDiff
    {
        private PortableElement pe;

        public DiffDel(PortableElement pe)
        {
            this.pe = pe;
        }

        public SyncResult SyncTo(BuildFile file)
        {
            var curr = this.pe.Current;

            if (file.TryGetElement(curr.Element, out XElement? actualElement))
            {
                actualElement.TryRemove();
                return SyncResult.Succeed;
            }

            return SyncResult.Failed;
        }
    }
}

namespace Mint.Substrate.Production
{
    using Mint.Substrate.Construction;

    public class DiffDel : BaseDiff
    {
        public DiffDel(ConvertibleElement element, PortingConfig config) : base(element, config) { }

        public override SyncResult Apply(NetCoreProjectFile file)
        {
            // if this element should be remove.
            if (this.Element.ConvertResult == ConvertResult.Removed)
            {
                return SyncResult.Succeed;
            }
            // is this element can be produced, try delete it.
            else
            {
                return file.RemoveElements(this.Element.ProducedElements);
            }
        }
    }
}

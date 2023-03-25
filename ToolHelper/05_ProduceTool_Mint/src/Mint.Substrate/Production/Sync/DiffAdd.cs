namespace Mint.Substrate.Production
{
    using System.Linq;
    using Mint.Substrate.Construction;

    public class DiffAdd : BaseDiff
    {
        public DiffAdd(ConvertibleElement element, PortingConfig config) : base(element, config) { }

        public override SyncResult Apply(NetCoreProjectFile file)
        {
            // if this element is removed after produce.
            if (this.Element.ConvertResult == ConvertResult.Removed)
            {
                return SyncResult.Succeed;
            }

            // use this element's last sibling as an anchor
            // if the anchor can be produced and result is not remove
            // we should find it in the project
            else
            {
                var lastElement = this.Element.LastSibling;
                if (lastElement != null)
                {
                    var anchorPortable = ConvertibleElement.Parse(lastElement, this._config);
                    if (anchorPortable.ConvertResult != ConvertResult.Removed)
                    {
                        var anchor = anchorPortable.ProducedElements.Last();
                        return file.AddElements(this.Element.ProducedElements, anchor);
                    }
                }
                return file.AddElements(this.Element.ProducedElements);
            }
        }
    }
}

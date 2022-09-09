namespace Mint.Substrate.Construction
{
    using System.Collections.Generic;
    using System.Xml.Linq;
    using Mint.Substrate.Production;

    public interface IModifier<T> where T : NetCoreProjectFile
    {
        SyncResult RemoveElements(List<XElement> elements);

        SyncResult AddElementsAfter(XElement anchor, List<XElement> elements);

        SyncResult AddElements(List<XElement> elements);
    }
}

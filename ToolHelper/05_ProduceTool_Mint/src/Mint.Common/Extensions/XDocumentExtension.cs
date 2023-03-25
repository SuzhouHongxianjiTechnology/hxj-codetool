namespace Mint.Common
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    public static class XDocumentExtension
    {
        public static void RemoveNamespace(this XDocument document)
        {
            foreach (var node in document.Root.DescendantsAndSelf())
            {
                node.Name = node.Name.LocalName;
            }
        }

        public static XElement GetFirst(this XDocument document, string tag)
        {
            return document.GetAll(tag).FirstOrDefault();
        }

        public static List<XElement> GetAll(this XDocument document, string tag)
        {
            return document.Descendants().Where(e => StringUtils.EqualsIgnoreCase(tag, e.Name.LocalName)).ToList();
        }
    }
}

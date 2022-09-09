namespace Mint.Common.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    public static class XDocumentExtension
    {
        /// <summary>
        /// Removes all the namespaces of this document and its descendants.
        /// </summary>
        public static void RemoveNamespace(this XDocument document)
        {
            document.Root.DescendantsAndSelf().ToList().ForEach(e => e.Name = e.Name.LocalName);
        }

        /// <summary>
        /// Gets the first descendant that matches the tag name.
        /// The comparison process is case-insensitive.
        /// </summary>
        public static XElement GetFirst(this XDocument document, string tag)
        {
            return document.GetAll(tag).FirstOrDefault();
        }

        /// <summary>
        /// Gets the last descendant that matches the tag name.
        /// The comparison process is case-insensitive.
        /// </summary>
        public static XElement GetLast(this XDocument document, string tag)
        {
            return document.GetAll(tag).LastOrDefault();
        }

        /// <summary>
        /// Gets all descendants that match the tag name.
        /// The comparison process is case-insensitive.
        /// </summary>
        public static IEnumerable<XElement> GetAll(this XDocument document, string tag)
        {
            return document.Descendants().Where(e => e.Name.LocalName.EqualsIgnoreCase(tag));
        }

        /// <summary>
        /// Gets all descendants that match the tags name.
        /// The comparison process is case-insensitive.
        /// </summary>
        public static IEnumerable<XElement> GetAll(this XDocument document, params string[] tags)
        {
            var result = new List<XElement>();
            tags.ToList().ForEach(tag => result.AddRange(document.GetAll(tag)));
            return result;
        }
    }
}

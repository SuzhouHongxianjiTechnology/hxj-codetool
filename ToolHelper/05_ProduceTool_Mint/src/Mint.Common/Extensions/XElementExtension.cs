namespace Mint.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    public static class XElementExtension
    {
        public static XElement Clone(this XElement element)
        {
            return XElement.Parse(element.ToString());
        }

        public static void RemoveNamespace(this XElement element)
        {
            foreach (var node in element.DescendantsAndSelf())
            {
                node.Name = node.Name.LocalName;
            }
        }

        public static bool Is(this XElement element, string tag)
        {
            return StringUtils.EqualsIgnoreCase(tag, element.Name.LocalName);
        }

        public static bool TryRemove(this XElement element)
        {
            try
            {
                element.Remove();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void RemoveIfEmpty(this XElement element)
        {
            if (!element.Elements().Any())
            {
                element.TryRemove();
            }
        }

        public static XElement GetFirst(this XElement element, string tag)
        {
            return element.GetAll(tag).FirstOrDefault();
        }

        public static List<XElement> GetAll(this XElement element, string tag)
        {
            return element.Elements().Where(e => StringUtils.EqualsIgnoreCase(tag, e.Name.LocalName)).ToList();
        }

        public static XElement LastSibling(this XElement element)
        {
            return element.ElementsBeforeSelf().LastOrDefault();
        }

        public static XElement NextSibling(this XElement element)
        {
            return element.ElementsAfterSelf().FirstOrDefault();
        }

        public static XAttribute GetAttribute(this XElement element, string attrName)
        {
            return element.Attributes().Where(a => StringUtils.EqualsIgnoreCase(attrName, a.Name.LocalName)).FirstOrDefault();
        }

        public static bool HasAttribute(this XElement element, string attrName)
        {
            XAttribute attr = element.GetAttribute(attrName);
            return attr != null;
        }

        public static bool HasAttribute(this XElement element, string attrName, string attrValue)
        {
            XAttribute attr = element.GetAttribute(attrName);
            if (attr != null)
            {
                return StringUtils.EqualsIgnoreCase(attrValue, attr.Value);
            }
            return false;
        }

        public static void SortByAttribute(this XElement parent, string attribute)
        {
            var children = parent.Elements().ToList();
            children.Sort((x, y) =>
                string.Compare(x.GetAttribute(attribute)?.Value,
                               y.GetAttribute(attribute)?.Value,
                               StringComparison.OrdinalIgnoreCase)
            );
            parent.RemoveNodes();
            foreach (XElement item in children) parent.Add(item);
        }
    }
}

namespace Mint.Substrate.Porting
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Mint.Common.Extensions;
    using Mint.Substrate.Construction;

    public class BuildFileComparer
    {
        private ElementConverter converter;

        public BuildFileComparer(ElementConverter converter)
        {
            this.converter = converter;
        }

        public List<IDiff> Compare(BuildFile fileA, BuildFile fileB)
        {
            var aElements = GetPortableElements(fileA);
            var bElements = GetPortableElements(fileB);

            var addElements = aElements.Except(bElements).ToList();
            var delElements = bElements.Except(aElements).ToList();

            var diffs = new List<IDiff>();

            delElements.ForEach(e => diffs.Add(new DiffDel(e)));
            addElements.ForEach(e => diffs.Add(new DiffAdd(e)));

            return diffs;
        }

        private List<PortableElement> GetPortableElements(BuildFile file)
        {
            var elements = new List<PortableElement>();

            PreConvertedElement? last = null, curr = null, next = null;

            var leafs = GetLeafs(file);

            for (int i = 0; i < leafs.Count; i++)
            {
                next = this.converter.Convert(leafs[i]);

                if (next?.ConvertResult == ConvertResult.Removed)
                { continue; }

                if (curr == null)
                { curr = next; continue; }

                elements.Add(new PortableElement(curr, last, next));
                { last = curr; curr = next; }
            }

            if (curr != null)
            {
                elements.Add(new PortableElement(curr, last, null));
            }

            return elements;
        }

        private List<XElement> GetLeafs(BuildFile file)
        {
            var result = new List<XElement>();
            foreach (var e in file.Document.Root.Elements().ToList())
            {
                if (e.Is(Tags.PropertyGroup) || e.Is(Tags.ItemGroup))
                {
                    foreach (var c in e.Elements())
                    {
                        result.Add(c);
                    }
                }
                else
                {
                    result.Add(e);
                }
            }
            return result;
        }

    }
}

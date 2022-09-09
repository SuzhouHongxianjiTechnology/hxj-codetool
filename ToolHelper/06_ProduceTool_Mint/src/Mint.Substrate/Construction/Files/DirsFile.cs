namespace Mint.Substrate.Construction
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Mint.Common.Extensions;

    public class DirsFile : XMLFile
    {
        private Dictionary<XElement, Dictionary<XNode, string>> groups;

        public IEnumerable<string> Entries => this.groups.SelectMany(kv => kv.Value).Select(i => i.Value);

        public DirsFile(string path) : base(path)
        {
            this.groups = new Dictionary<XElement, Dictionary<XNode, string>>();
            foreach (var itemGroup in this.Document.GetAll(Tags.ItemGroup))
            {
                foreach (var node in itemGroup.Nodes().OfType<XElement>())
                {
                    if (node.Is(Tags.ProjectFile) && node.HasAttribute(Tags.Include))
                    {
                        string dir = node.GetAttribute(Tags.Include).Value;
                        if (!this.groups.ContainsKey(itemGroup))
                        {
                            this.groups.Add(itemGroup, new Dictionary<XNode, string>());
                        }
                        this.groups[itemGroup].Add(node, dir);
                    }
                }

                foreach (var node in itemGroup.Nodes().OfType<XComment>())
                {
                    try
                    {
                        var element = XElement.Parse(node.Value);
                        if (element.Is(Tags.ProjectFile) && element.HasAttribute(Tags.Include))
                        {
                            string dir = element.GetAttribute(Tags.Include).Value;
                            if (!this.groups.ContainsKey(itemGroup))
                            {
                                this.groups.Add(itemGroup, new Dictionary<XNode, string>());
                            }
                            this.groups[itemGroup].Add(node, dir);
                        }
                    }
                    catch { }
                }
            }
        }

        public void RemovePath(string path)
        {
            foreach (var nodes in this.groups.Values)
            {
                foreach (var node in nodes)
                {
                    if (node.Value.EqualsIgnoreCase(path))
                    {
                        node.Key.Remove();
                        nodes.Remove(node.Key);
                    }
                }
            }
        }

        public void AddPathAfter(string anchor, string path)
        {
            foreach (var group in this.groups)
            {
                if (!group.Value.Values.Contains(anchor)) continue;

                foreach (var item in group.Value.Keys.ToList())
                {
                    if (group.Value[item].EqualsIgnoreCase(anchor))
                    {
                        var e = new XElement(
                            Namespace + Tags.ProjectFile,
                            new XAttribute(Tags.Include, path)
                        );

                        item.AddAfterSelf(e);
                        group.Value.Add(e, path);
                    }
                }
            }
        }
    }
}

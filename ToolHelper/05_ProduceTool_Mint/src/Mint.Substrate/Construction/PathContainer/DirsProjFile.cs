namespace Mint.Substrate.Construction
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Mint.Common;

    public sealed class DirsProjFile : PathContainer
    {
        internal protected override bool ReadOnly => false;

        public DirsProjFile(string path) : base(path) { }

        public override void AddPath(string path)
        {
            string anchor = path.Replace(".NetCore", "")
                                .Replace(".NetStd", "");

            XElement element = new XElement(this.Namespace + Tags.ProjectFile,
                new XAttribute(Tags.Include, path)
            );

            if (this.TryFindNodesWithPath(anchor, out List<XNode> nodes))
            {
                nodes.ForEach(node => node.AddAfterSelf(element));
            }
            else
            {
                this.Document.GetFirst(Tags.ItemGroup).AddFirst(element);
            }
        }

        public override void RemovePath(string path)
        {
            if (this.TryFindNodesWithPath(path, out List<XNode> nodes))
            {
                nodes.ForEach(node => node.Remove());
            }
        }

        private bool TryFindNodesWithPath(string path, out List<XNode> nodes)
        {
            nodes = new List<XNode>();

            foreach (var element in this.Document.GetAll(Tags.ProjectFile))
            {
                if (element.HasAttribute(Tags.Include, path))
                {
                    nodes.Add(element);
                }
            }

            foreach (var comment in this.Document.Root.DescendantNodes().OfType<XComment>())
            {
                try
                {
                    var element = XElement.Parse(comment.Value);
                    if (element.HasAttribute(Tags.Include, path))
                    {
                        nodes.Add(element);
                    }
                }
                catch { }
            }

            return nodes.Any();
        }
    }
}

namespace Mint.Substrate.Construction
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Mint.Common;
    using Mint.Substrate.Constants;
    using Mint.Substrate.Production;

    internal sealed class DefaultComparator : IComparator<NetFrameworkProjectFile>
    {
        private List<ConvertibleElement> LoadElements(NetFrameworkProjectFile file, PortingConfig config)
        {
            file.Document.RemoveNamespace();
            var elements = new List<XElement>();
            XElement root = file.Document.Root;
            elements.AddRange(
                root.GetAll(Tags.PropertyGroup)
                    .SelectMany(p => p.Elements())
                    .Where(p => !NetFrameworkConsts.NetFrameworkOnylProperties.Contains(p.Name.LocalName))
            );
            elements.AddRange(
                root.GetAll(Tags.ItemGroup)
                    .SelectMany(p => p.Elements())
            );
            elements.AddRange(
                root.GetAll(Tags.Target)
            );
            elements.AddRange(
                root.GetAll(Tags.Import)
            );
            return elements.Select(e => ConvertibleElement.Parse(e, config))
                           .ToList();
        }

        public List<BaseDiff> CompareTo(NetFrameworkProjectFile projectA, NetFrameworkProjectFile projectB, PortingConfig config)
        {
            var aElements = this.LoadElements(projectA, config);
            var bElements = this.LoadElements(projectB, config);

            var addElements = aElements.Except(bElements).ToList();
            var delElements = bElements.Except(aElements).ToList();

            var diffs = new List<BaseDiff>();
            // order here is very importent!
            delElements.ForEach(e => diffs.Add(new DiffDel(e, config)));
            addElements.ForEach(e => diffs.Add(new DiffAdd(e, config)));

            return diffs;
        }
    }
}

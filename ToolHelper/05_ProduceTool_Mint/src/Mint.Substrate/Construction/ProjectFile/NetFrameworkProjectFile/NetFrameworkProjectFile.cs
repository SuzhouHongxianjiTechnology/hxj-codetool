namespace Mint.Substrate.Construction
{
    using System.Collections.Generic;
    using System.Linq;
    using Mint.Common;
    using Mint.Substrate.Production;

    public class NetFrameworkProjectFile : ProjectFile
    {
        internal protected override bool ReadOnly => true;

        public string AssemblyName { get; }

        public List<NetFrameworkReference> References { get; }

        public List<NetFrameworkProjectReference> ProjectReferences { get; }

        public NetFrameworkProjectFile(string path) : base(path)
        {
            this.AssemblyName = this.Document.GetFirst(Tags.AssemblyName)?.Value;

            this.References = this.Document.GetAll(Tags.Reference)
                                           .Select(item => new NetFrameworkReference(item))
                                           .ToList();

            this.ProjectReferences = this.Document.GetAll(Tags.ProjectReference)
                                                  .Select(item => new NetFrameworkProjectReference(item, this.ParentPath))
                                                  .ToList();
        }

        public List<BaseDiff> CompareTo(NetFrameworkProjectFile other, PortingConfig config, IComparator<NetFrameworkProjectFile> comparator = null)
        {
            comparator = comparator ?? new DefaultComparator();
            return comparator.CompareTo(this, other, config);
        }
    }
}

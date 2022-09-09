namespace Mint.Substrate.Construction
{
    using System.IO;
    using System.Xml.Linq;
    using Mint.Common;

    public class NetFrameworkProjectReference : ProjectElement
    {
        public string Name { get; }

        public NetFrameworkProjectReference(XElement element, string parentPath) : base(element)
        {
            string include = this.Element.GetAttribute(Tags.Include)?.Value;
            string projectPath = Path.GetFullPath(include, parentPath);
            if (File.Exists(projectPath))
            {
                this.Name = new NetFrameworkProjectFile(projectPath).AssemblyName;
            }
            else
            {
                this.Name = this.Element.GetFirst(Tags.Name)?.Value;
            }
        }
    }
}

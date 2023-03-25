namespace Mint.Substrate.Construction
{
    using System.IO;
    using System.Xml.Linq;
    using Mint.Common;

    public abstract class ProjectFile : XmlFile
    {
        public string FileName { get; }

        public string ParentPath { get; }

        public ProjectFile(string path) : base(path)
        {
            this.FileName = Path.GetFileName(path);
            this.ParentPath = Directory.GetParent(path).FullName;
        }

        public XElement GetProperty(string tag)
        {
            return this.Document.GetFirst(tag);
        }
    }
}

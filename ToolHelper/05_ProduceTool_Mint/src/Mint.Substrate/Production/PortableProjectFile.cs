namespace Mint.Substrate.Production
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Mint.Common;
    using Mint.Substrate.Construction;

    public sealed class PortableProjectFile : ProjectFile
    {
        internal protected override bool ReadOnly => false;

        public PortableProjectFile(string path) : base(path) { }

        // ----------------------------------------------------------------

        public void Produce(PortingConfig config)
        {
            // Remove xml namespace
            this.Document.RemoveNamespace();

            // Cleanup root
            var root = this.Document.Root;
            root.Attributes().Remove();
            root.Add(new XAttribute(Tags.Sdk, "Microsoft.NET.Sdk"));

            // Produce Elements
            foreach (var element in root.Elements().ToList())
            {
                ConvertibleElement.Parse(element, config).Produce();
                element.RemoveNamespace();
            }
        }
    }
}

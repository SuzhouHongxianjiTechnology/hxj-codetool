namespace Mint.Substrate.Porting
{
    using System.Linq;
    using System.Xml.Linq;
    using Mint.Common.Extensions;
    using Mint.Substrate.Construction;

    public class BuildFileProducer
    {
        private readonly ElementConverter converter;

        public BuildFileProducer(ElementConverter converter)
        {
            this.converter = converter;
        }

        public void Produce(BuildFile file)
        {
            // remove xml namespace
            file.Document.RemoveNamespace();

            // cleanup root
            var root = file.Document.Root;
            root.Attributes().Remove();
            root.Add(new XAttribute(Tags.Sdk, "Microsoft.NET.Sdk"));

            foreach (var element in file.Document.Root.Elements().ToList())
            {
                this.converter.Convert(element)?.Commit();
                element.RemoveNamespace();
            }
        }
    }
}

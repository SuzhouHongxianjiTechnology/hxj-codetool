namespace Mint.Substrate.Construction
{
    using System.Xml;
    using System.Xml.Linq;
    using Mint.Substrate.Utilities;

    public class XMLFile
    {
        public string FilePath { get; }

        public XDocument Document { get; }

        public XNamespace Namespace { get; }

        public XMLFile(string path)
        {
            ErrorUtils.VerifyThrowFileNotExists(path);
            ErrorUtils.VerifyThrowXmlLoadFail(path);

            this.FilePath = path;
            this.Document = XDocument.Load(path);
            this.Namespace = this.Document.Root.GetDefaultNamespace();
        }

        public void Save(bool omniDeclaration = false)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = omniDeclaration,
                Indent = true
            };

            using (var writer = XmlWriter.Create(FilePath, settings))
            {
                this.Document.Save(writer);
            }
        }
    }
}

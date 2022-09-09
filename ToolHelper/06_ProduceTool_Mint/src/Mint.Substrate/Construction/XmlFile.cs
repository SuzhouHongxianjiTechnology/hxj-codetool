namespace Mint.Substrate.Construction
{
    using System;
    using System.Xml;
    using System.Xml.Linq;
    using Mint.Substrate.Utilities;

    public abstract class XmlFile : IDisposable
    {
        internal protected abstract bool ReadOnly { get; }

        internal protected virtual bool OmitXmlDeclaration { get; } = false;

        internal XDocument Document { get; }

        internal XNamespace Namespace { get; }

        public string FullPath { get; }

        public XmlFile(string path)
        {
            Verification.VerifyThrowFileNotExists(path);
            this.FullPath = path;

            Verification.VerifyThrowXmlLoadFail(path);
            this.Document = XDocument.Load(path);

            this.Namespace = this.Document.Root.GetDefaultNamespace();
        }

        public void Dispose()
        {
            if (this.ReadOnly) return;

            XmlWriterSettings settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = this.OmitXmlDeclaration,
                Indent = true
            };

            using (XmlWriter writer = XmlWriter.Create(this.FullPath, settings))
            {
                this.Document.Save(writer);
            }
        }
    }
}

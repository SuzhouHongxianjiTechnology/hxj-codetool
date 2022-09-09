namespace Mint.Substrate.Construction
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;
    using Mint.Common;
    using Mint.Substrate.Production;

    public class NetCoreProjectFile : ProjectFile
    {
        private IFormatter<NetCoreProjectFile> _formatter;

        private IModifier<NetCoreProjectFile> _modifier;

        internal protected override bool ReadOnly => false;

        internal protected override bool OmitXmlDeclaration => true;

        public string AssemblyName { get; }

        public string TargetPath { get; }

        public List<NetCoreReference> References { get; }

        public bool ContainsVariantConfig { get; }

        public List<NetCorePackageReference> PackageReferences { get; }

        public NetCoreProjectFile(string path, IFormatter<NetCoreProjectFile> formatter = null, IModifier<NetCoreProjectFile> modifier = null) : base(path)
        {
            this._formatter = formatter ?? new DefaultFormatter(this);

            this._modifier = modifier ?? new DefaultModifier(this);

            this.AssemblyName = this.Document.GetFirst(Tags.AssemblyName)?.Value;

            {
                // full path:
                // eg: D:\xxx\xxx\sources\dev\assistants\src\Assistants.NetCore\Microsoft.Exchange.Assistants.NetCore.csproj

                // dev\assistants\src\Assistants.NetCore\Microsoft.Exchange.Assistants.NetCore.csproj
                string devPath = this.FullPath.Split(@"sources\").Last();

                // dev\assistants\
                string rootPath = Regex.Split(devPath, "src", RegexOptions.IgnoreCase).First();

                // Microsoft.Exchange.Assistants.NetCore
                string folderName = Path.GetFileNameWithoutExtension(this.FullPath);

                // $(TargetPathDir)dev\assistants\Microsoft.Exchange.Assistants.NetCore\$(FlavorPlatformDir)\{assemblyName}.dll
                this.TargetPath = $"$(TargetPathDir){rootPath}{folderName}\\$(FlavorPlatformDir)\\{this.AssemblyName}.dll";
            }

            {
                this.References = this.Document.GetAll(Tags.Reference)
                                               .Select(item => new NetCoreReference(item))
                                               .ToList();
            }

            {
                this.ContainsVariantConfig = this.Document.GetAll(Tags.Import)
                                                          .Where(item => item.HasAttribute(Tags.Project))
                                                          .Select(item => item.GetAttribute(Tags.Project).Value.Split("\\").Last())
                                                          .Where(targets => StringUtils.EqualsIgnoreCase(targets, "VariantConfiguration.targets"))
                                                          .Any();
            }

            {
                this.PackageReferences = this.Document.GetAll(Tags.PackageReference)
                                                      .Select(item => new NetCorePackageReference(item))
                                                      .ToList();
            }
        }

        // ----------------------------------------------------------------

        public void Format(IFormatter<NetCoreProjectFile> formatter = null)
        {
            formatter = formatter ?? this._formatter;
            formatter.Format();
        }

        public SyncResult AddElements(List<XElement> elements, XElement anchor = null)
        {
            if (anchor != null)
                return this._modifier.AddElementsAfter(anchor, elements);
            else
                return this._modifier.AddElements(elements);

        }

        public SyncResult RemoveElements(List<XElement> elements)
        {
            return this._modifier.RemoveElements(elements);
        }
    }
}

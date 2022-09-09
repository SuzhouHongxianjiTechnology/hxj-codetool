namespace Mint.Substrate.Construction
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using Mint.Common.Extensions;

    public class BuildFile : XMLFile
    {
        public string AssemblyName { get; }

        public bool ContainsVariantConfigV1 => this.Document.GetAll(Tags.Import)
                                                            .Where(i => i.HasAttribute(Tags.Project))
                                                            .Select(i => i.GetAttribute(Tags.Project).Value.Split("\\").Last())
                                                            .Where(p => p.EqualsIgnoreCase("VariantConfiguration.targets"))
                                                            .Any();

        public bool ContainsVariantConfigV2 => this.Document.GetAll(Tags.Import)
                                                            .Where(i => i.HasAttribute(Tags.Project))
                                                            .Select(i => i.GetAttribute(Tags.Project).Value.Split("\\").Last())
                                                            .Where(p => p.EqualsIgnoreCase("VariantConfigurationClient.targets"))
                                                            .Any();

        public bool ContainsPerfCounters => this.Document.GetAll(Tags.PerfCounter).Any();

        public BuildFile(string path) : base(path)
        {
            string? assemblyName = this.Document.GetFirst(Tags.AssemblyName)?.Value;

            if (string.IsNullOrEmpty(assemblyName))
            {
                string fileName = Path.GetFileNameWithoutExtension(path);
                assemblyName = fileName;
            }
            else if (assemblyName.EqualsIgnoreCase(@"$(RootNamespace)"))
            {
                assemblyName = this.Document.GetFirst(Tags.RootNamespace).Value;
            }

            this.AssemblyName = assemblyName;
        }

        public ReferenceSet GetReferences(ReferenceResolver resolver)
        {
            var refs = this.Document.GetAll(Tags.Reference, Tags.ProjectReference, Tags.PackageReference);
            return new ReferenceSet(refs, resolver);
        }

        public bool TryGetElement(XElement? element, [MaybeNullWhen(false)] out XElement actualElement)
        {
            throw new System.NotImplementedException();
        }

        public bool TryAddElement(XElement? element)
        {
            throw new System.NotImplementedException();
        }
    }
}

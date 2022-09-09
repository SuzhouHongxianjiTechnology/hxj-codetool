namespace Mint.Substrate.Construction
{
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;
    using Mint.Common.Extensions;
    using Mint.Common.Utilities;
    using Mint.Substrate.Utilities;

    public class ReferenceResolver
    {
        private readonly string filePath;

        private readonly LookupTable lookupTable;

        private readonly Regex pkgPattern;

        public ReferenceResolver(string filePath, LookupTable lookupTable)
        {
            this.filePath = filePath;
            this.lookupTable = lookupTable;
            this.pkgPattern = new Regex(@"\$\(Pkg(.*?)([_\d]*)\).*");
        }

        public IReference Resolve(in XElement element)
        {
            string? actualName;
            string refName, dllName;

            if (element.Is(Tags.ProjectReference))
            {
                ResolveProjectReference(element, out refName, out dllName);
                return new Reference(refName, dllName, ReferenceType.Substrate);
            }

            else if (element.Is(Tags.PackageReference))
            {
                ResolvePackageReference(element, out refName, out dllName);
                return new Reference(refName, dllName, ReferenceType.NuGet);
            }

            else
            {
                ResolveNormalReference(element, out refName, out dllName, out bool unnecessary);

                // 1. try as SDK
                if (this.lookupTable.IsSDK(refName, out actualName))
                {
                    return new Reference(actualName, dllName, ReferenceType.SDK, unnecessary);
                }

                // 2. try as NuGet
                if (this.lookupTable.IsDefinedNuGet(refName, out actualName, out _) ||
                    this.lookupTable.IsKnownNuGet(refName, out actualName))
                {
                    return new Reference(actualName, dllName, ReferenceType.NuGet, unnecessary);
                }

                // any reference starts with $(Pkg is a package reference
                MatchCollection matchs = this.pkgPattern.Matches(element.ToString());
                if (matchs.Any())
                {
                    string packageName = matchs[0].Groups[1].Value.Replace("_", ".");
                    return new Reference(packageName, dllName, ReferenceType.NuGet, unnecessary);
                }

                // 3. try as normal project
                if (this.lookupTable.IsProducedProject(refName, out IProject? project))
                {
                    string assemblyName = project.Name;
                    return new Reference(assemblyName, $"{assemblyName}.dll", ReferenceType.Substrate, unnecessary);
                }
                else
                {
                    return new Reference(refName, $"{refName}.dll", ReferenceType.Substrate, unnecessary);
                }
            }
        }

        private void ResolveProjectReference(in XElement element, out string refName, out string dllName)
        {
            string include = element.GetAttribute(Tags.Include).Value;
            string parent = Directory.GetParent(this.filePath).FullName;
            MSBuildUtils.TryResolveBuildVariables(parent, include, out string normalPath);
            string fullPath = PathUtils.GetAbsolutePath(parent, normalPath);

            if (File.Exists(fullPath))
            {
                refName = Repo.Load<BuildFile>(fullPath).AssemblyName;
                dllName = $"{refName}.dll";
            }
            else
            {
                string errMsg = $"Can not resolve ProjectReference: '{include}' in '{this.filePath}', last resolve result: {fullPath}";
                throw new FileNotFoundException(errMsg);
            }
        }

        private void ResolvePackageReference(in XElement element, out string refName, out string dllName)
        {
            string include = element.GetAttribute(Tags.Include).Value;
            refName = include;
            dllName = $"{refName}.dll";
        }

        private void ResolveNormalReference(in XElement element, out string refName, out string dllName, out bool unnecessary)
        {
            refName = string.Empty;
            dllName = string.Empty;
            unnecessary = false;

            string include = element.GetAttribute(Tags.Include).Value; // Assert any reference SHOULD have Include
            var hintPath = element.GetFirst(Tags.HintPath);

            /*
                References with no hintpath could be:
                a) SDK references, which has name only, we'll have to guess it's dll name
                b) Other type references, it's include SHOULD be the dll path, we'll have guess it's assembly name
            */
            if (hintPath == null)
            {
                if (include.EndsWithIgnoreCase("dll") || include.EndsWithIgnoreCase("exe"))
                {
                    refName = Path.GetFileNameWithoutExtension(include);
                    dllName = Path.GetFileName(include);
                }
                else
                {
                    refName = include;
                    dllName = $"{include}.dll";
                }
            }

            /*
                References with hintpath normally has it's name in 'Include'
            */
            else
            {
                if (include.EndsWithIgnoreCase("dll") || include.EndsWithIgnoreCase("exe"))
                {
                    refName = Path.GetFileNameWithoutExtension(include);
                }
                else
                {
                    refName = include;
                }
                dllName = Path.GetFileName(hintPath.Value);
            }

            if (element.HasAttribute(Tags.AllowedUnnecessary, "true"))
            {
                unnecessary = true;
            }
            else
            {
                var au = element.GetFirst(Tags.AllowedUnnecessary);
                unnecessary = au != null && au.Value.EqualsIgnoreCase("true");
            }
        }
    }
}

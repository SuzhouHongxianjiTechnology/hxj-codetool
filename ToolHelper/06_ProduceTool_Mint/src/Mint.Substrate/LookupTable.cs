namespace Mint.Substrate
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Mint.Common.Collections;
    using Mint.Substrate.Configurations;
    using Mint.Substrate.Construction;
    using Mint.Substrate.LookupTables;

    public class LookupTable
    {
        private readonly PropertyLookupTable propTable;
        private readonly NuGetLookupTable refsTable;
        private readonly ProjectLookupTable projTable;

        public LookupTable()
        {
            this.propTable = new PropertyLookupTable(SubstrateProperties.Settings);
            this.refsTable = new NuGetLookupTable(SubstrateNuGets.Settings);
            this.projTable = new ProjectLookupTable(SubstrateProjects.Settings);
        }

        #region Properties

        public HashSet<string> KnownProps => this.propTable.KnownProps;

        public Dictionary<string, string> IncludedProps => this.propTable.IncludedProps;

        public HashSet<string> ExcludedProps => this.propTable.ExcludedProps;

        public bool IsProperty(string property)
        {
            return this.propTable.KnownProps.Contains(property);
        }

        public bool IsIncludedProperty(string property, [MaybeNullWhen(false)] out string defaultValue)
        {
            return this.propTable.IncludedProps.TryGetValue(property, out defaultValue);
        }

        public bool IsExcludedProperty(string property)
        {
            return this.propTable.ExcludedProps.Contains(property);
        }

        #endregion


        #region References

        public IgnoreCaseDictionary<string> DefinedNuGets => this.refsTable.DefinedNuGets;
        public HashSet<string> VarConfigV1Refs => this.refsTable.VarConfigV1Refs;
        public HashSet<string> VarConfigV2Refs => this.refsTable.VarConfigV2Refs;
        public HashSet<string> KnownSDKs => this.refsTable.KnownSDKs;
        public HashSet<string> ExcludedNuGets => this.refsTable.ExcludedNuGets;

        public bool IsReplaceNuGet(string name, [MaybeNullWhen(false)] out string replaceName)
        {
            return this.refsTable.Replacement.TryGetValue(name, out replaceName);
        }

        public bool IsDefinedNuGet(string name, [MaybeNullWhen(false)] out string actualName, [MaybeNullWhen(false)] out string version)
        {
            actualName = version = null;
            return this.DefinedNuGets.TryGetKey(name, out actualName) &&
                   this.DefinedNuGets.TryGetValue(name, out version);
        }

        public bool IsDefinedInVariantConfigV1(string nuget)
        {
            return this.refsTable.VarConfigV1Refs.Contains(nuget);
        }

        public bool IsDefinedInVariantConfigV2(string nuget)
        {
            return this.refsTable.VarConfigV2Refs.Contains(nuget);
        }

        public bool IsSDK(string name, [MaybeNullWhen(false)] out string actualName)
        {
            return this.refsTable.KnownSDKs.TryGetValue(name, out actualName);
        }

        public bool IsKnownNuGet(string name, [MaybeNullWhen(false)] out string actualName)
        {
            return this.refsTable.KnownNuGets.TryGetValue(name, out actualName);
        }

        public bool IsExcludedNuGet(string name, [MaybeNullWhen(false)] out string actualName)
        {
            return this.ExcludedNuGets.TryGetValue(name, out actualName);
        }

        #endregion


        #region Projects

        public bool IsProducedProject(string assemblyName, [MaybeNullWhen(false)] out IProject project)
        {
            return this.projTable.ProducedProjects.TryGetProject(assemblyName, out project);
        }

        public bool IsCppProject(string assemblyName, [MaybeNullWhen(false)] out string actualName)
        {
            return this.projTable.CppProjects.TryGetValue(assemblyName, out actualName);
        }

        public ProjectSet GetProducedProjects()
        {
            return this.projTable.ProducedProjects;
        }

        #endregion
    }
}

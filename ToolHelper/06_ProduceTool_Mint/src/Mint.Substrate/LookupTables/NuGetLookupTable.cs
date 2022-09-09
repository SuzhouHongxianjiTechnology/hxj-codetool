namespace Mint.Substrate.LookupTables
{
    using System;
    using System.Collections.Generic;
    using Mint.Common.Collections;
    using Mint.Substrate.Configurations;

    internal class NuGetLookupTable
    {
        private SubstrateNuGets repoNuGets;

        private IgnoreCaseDictionary<string>? definedNuGets;

        private Dictionary<string, string>? replacement;

        private HashSet<string>? varConfigV1Refs;

        private HashSet<string>? varConfigV2Refs;

        private HashSet<string>? knownSDKs;

        private HashSet<string>? knownNuGets;

        private HashSet<string>? excludedNuGets;

        internal IgnoreCaseDictionary<string> DefinedNuGets
        {
            get
            {
                if (this.definedNuGets == null)
                {
                    var nuGets = Repo.PackagesProps.GetPackages().AsDictionary();
                    this.definedNuGets = new IgnoreCaseDictionary<string>(nuGets);
                }
                return this.definedNuGets;
            }
        }

        internal Dictionary<string, string> Replacement
        {
            get
            {
                if (this.replacement == null)
                {
                    this.replacement = new Dictionary<string, string>(this.repoNuGets.NuGetReplacement, StringComparer.OrdinalIgnoreCase);
                }
                return this.replacement;
            }
        }

        internal HashSet<string> VarConfigV1Refs
        {
            get
            {
                if (this.varConfigV1Refs == null)
                {
                    this.varConfigV1Refs = Repo.VarConfigV1.PackageReferences;
                }
                return this.varConfigV1Refs;
            }
        }

        internal HashSet<string> VarConfigV2Refs
        {
            get
            {
                if (this.varConfigV2Refs == null)
                {
                    this.varConfigV2Refs = Repo.VarConfigV2.PackageReferences;
                }
                return this.varConfigV2Refs;
            }
        }

        internal HashSet<string> KnownSDKs
        {
            get
            {
                if (this.knownSDKs == null)
                {
                    this.knownSDKs = new HashSet<string>(this.repoNuGets.KnownSDKs, StringComparer.OrdinalIgnoreCase);
                }
                return this.knownSDKs;
            }
        }

        internal HashSet<string> KnownNuGets
        {
            get
            {
                if (this.knownNuGets == null)
                {
                    this.knownNuGets = new HashSet<string>(this.repoNuGets.KnownNuGets, StringComparer.OrdinalIgnoreCase);
                }
                return this.knownNuGets;
            }
        }

        internal HashSet<string> ExcludedNuGets
        {
            get
            {
                if (this.excludedNuGets == null)
                {
                    this.excludedNuGets = new HashSet<string>(this.repoNuGets.ExcludedNuGets, StringComparer.OrdinalIgnoreCase);
                }
                return this.excludedNuGets;
            }
        }

        internal NuGetLookupTable(SubstrateNuGets repoNuGets)
        {
            this.repoNuGets = repoNuGets;
        }
    }
}

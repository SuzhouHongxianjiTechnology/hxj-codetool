namespace Mint.Substrate.LookupTables
{
    using System;
    using System.Collections.Generic;
    using Mint.Substrate.Configurations;

    internal class PropertyLookupTable
    {
        private SubstrateProperties repoProps;

        private HashSet<string>? knownProps;

        private Dictionary<string, string>? includedProps;

        private HashSet<string>? excludedProps;

        internal HashSet<string> KnownProps
        {
            get
            {
                if (this.knownProps == null)
                {
                    this.knownProps = new HashSet<string>(this.repoProps.KnownProperties, StringComparer.OrdinalIgnoreCase);
                }
                return this.knownProps;
            }
        }

        internal Dictionary<string, string> IncludedProps
        {
            get
            {
                if (this.includedProps == null)
                {
                    this.includedProps = new Dictionary<string, string>(this.repoProps.IncludeProperties, StringComparer.OrdinalIgnoreCase);
                }
                return this.includedProps;
            }
        }

        internal HashSet<string> ExcludedProps
        {
            get
            {
                if (this.excludedProps == null)
                {
                    this.excludedProps = new HashSet<string>(this.repoProps.ExcludedProperties, StringComparer.OrdinalIgnoreCase);
                }
                return this.excludedProps;
            }
        }

        internal PropertyLookupTable(SubstrateProperties repoProps)
        {
            this.repoProps = repoProps;
        }
    }
}

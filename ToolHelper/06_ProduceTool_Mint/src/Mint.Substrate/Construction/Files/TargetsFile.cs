namespace Mint.Substrate.Construction
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Mint.Common.Extensions;

    public class TargetsFile : XMLFile
    {
        public HashSet<string> PackageReferences { get; }

        public TargetsFile(string path) : base(path)
        {
            this.PackageReferences = this.Document.GetAll(Tags.PackageReference)
                                                  .Select(r => r.GetAttribute(Tags.Include).Value)
                                                  .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }
    }
}

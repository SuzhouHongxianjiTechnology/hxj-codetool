namespace Mint.Substrate.Construction
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Xml.Linq;
    using Mint.Common.Collections;

    public class ReferenceSet : IEnumerable<IReference>
    {
        private IgnoreCaseDictionary<IReference> references = new IgnoreCaseDictionary<IReference>();

        public ReferenceSet(IEnumerable<XElement> refElements, ReferenceResolver resolver)
        {
            foreach (var refElement in refElements)
            {
                var reference = resolver.Resolve(refElement);
                this.references.TryAdd(reference.ReferenceName, reference);
            }
        }

        public bool Has(string name, [MaybeNullWhen(false)] out string actualName)
        {
            actualName = null;
            return this.references.TryGetKey(name, out actualName);
        }

        public IEnumerator<IReference> GetEnumerator()
        {
            foreach (var reference in this.references.Values)
            {
                yield return reference;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

namespace Mint.Substrate.Construction
{
    public class UnknownReference : IReference
    {
        public string ReferenceName => "Unknown";

        public string ReferenceDll => "Unknown.dll";

        public ReferenceType Type => ReferenceType.Unknown;

        public bool Unnecessary => false;
    }
}

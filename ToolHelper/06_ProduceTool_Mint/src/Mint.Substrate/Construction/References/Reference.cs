namespace Mint.Substrate.Construction
{
    public class Reference : IReference
    {
        public string ReferenceName { get; }

        public string ReferenceDll { get; }

        public ReferenceType Type { get; }

        public bool Unnecessary { get; }

        public Reference(string refName, string dllName, ReferenceType type, bool unnecessary = false)
        {
            this.ReferenceName = refName;
            this.ReferenceDll = dllName;
            this.Type = type;
            this.Unnecessary = unnecessary;
        }
    }
}

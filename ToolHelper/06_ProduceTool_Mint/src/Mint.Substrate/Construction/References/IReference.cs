namespace Mint.Substrate.Construction
{
    public interface IReference
    {
        string ReferenceName { get; }

        string ReferenceDll { get; }

        ReferenceType Type { get; }

        bool Unnecessary { get; }
    }
}

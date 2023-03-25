namespace Mint.Substrate.Construction
{
    public interface IFormatter<T> where T : NetCoreProjectFile
    {
        void Format();
    }
}

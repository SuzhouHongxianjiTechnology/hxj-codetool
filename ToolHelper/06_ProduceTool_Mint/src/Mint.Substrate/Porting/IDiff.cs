namespace Mint.Substrate.Porting
{
    using Mint.Substrate.Construction;

    public interface IDiff
    {
        SyncResult SyncTo(BuildFile file);
    }
}

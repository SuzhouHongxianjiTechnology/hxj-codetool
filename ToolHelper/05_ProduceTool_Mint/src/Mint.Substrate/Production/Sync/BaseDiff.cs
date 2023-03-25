namespace Mint.Substrate.Production
{
    using Mint.Substrate.Construction;

    public abstract class BaseDiff
    {
        protected ConvertibleElement Element;

        protected PortingConfig _config;

        protected BaseDiff(ConvertibleElement element, PortingConfig config)
        {
            this.Element = element;
            this._config = config;
        }

        public abstract SyncResult Apply(NetCoreProjectFile file);
    }
}

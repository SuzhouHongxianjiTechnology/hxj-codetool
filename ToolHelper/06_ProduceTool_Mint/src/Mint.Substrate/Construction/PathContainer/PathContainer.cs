namespace Mint.Substrate.Construction
{
    public abstract class PathContainer : XmlFile
    {
        public PathContainer(string path) : base(path) { }

        public abstract void AddPath(string path);

        public abstract void RemovePath(string path);
    }
}


namespace Scripts
{
    using System.Collections.Generic;
    using System.IO;
    using Mint.Substrate;
    using Mint.Substrate.Construction;

    public class Ceres
    {
        static LookupTable Table = new LookupTable();


        public static void FixBuildFile(string path)
        {
            var file = Repo.Load<BuildFile>(path);
            var root = Directory.GetParent(path);

            foreach (var csFile in WalkDirectoryTree(root))
            {

            }
        }

        public static IEnumerable<string> WalkDirectoryTree(DirectoryInfo root)
        {
            foreach (var source in root.GetFiles("*.cs"))
            {
                yield return source.ToString();
            }

            foreach (var dir in root.GetDirectories())
            {
                foreach (var source in WalkDirectoryTree(dir))
                {
                    yield return source;
                }
            }
        }
    }
}

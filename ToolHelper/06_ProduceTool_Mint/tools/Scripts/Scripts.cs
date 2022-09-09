
namespace Scripts
{
    using System.IO;
    using System.Linq;
    using Mint.Common.Extensions;
    using Mint.Common.Utilities;
    using Mint.Database.Remote;
    using Mint.Substrate;
    using Mint.Substrate.Construction;

    public class Scripts
    {
        static LookupTable Table = new LookupTable();

        public static void MapiFederatedDirectoryPaths()
        {
            using (Timer.TimeThis)
            {
                var foo = Neo4jQueries.GetProcessToTypePathsAsync("MapiHttp", "15.20.4280.000", "Microsoft.Exchange.FederatedDirectory.dll,Microsoft.Exchange.FederatedDirectory,UpdateSiteCollectionTask").Result;
                ConsoleLog.Debug(foo.Count);
                foreach (var result in foo)
                {
                    string ident = "";
                    foreach (var bar in result)
                    {
                        ConsoleLog.Highlight($"{ident}{bar}");
                        ident += "  ";
                    }
                }
            }
        }

        public static void RemovePerfCounters()
        {
            var produced = Table.GetProducedProjects();
            foreach (var project in produced)
            {
                if (!Path.GetExtension(project.FilePath).EqualsIgnoreCase(".csproj")) continue;

                var file = Repo.Load<BuildFile>(project.FilePath);
                bool edit = false;
                // <Import Project="$(MSBuildExtensionsPath)\Override\PerfCounters.targets" />
                var imports = file.Document.GetAll(Tags.Import);
                foreach (var import in imports.ToList())
                {
                    if (import.HasAttribute(Tags.Project, "$(MSBuildExtensionsPath)\\Override\\PerfCounters.targets"))
                    {
                        import.TryRemove();
                        edit = true;
                    }
                }
                if (edit)
                    file.Save(omniDeclaration: true);
            }
        }

        public static void RemoveDISTRIB_PRIVATE_INC_PATH()
        {
            var produced = Table.GetProducedProjects();
            foreach (var project in produced)
            {
                if (!Path.GetExtension(project.FilePath).EqualsIgnoreCase(".csproj")) continue;

                var file = Repo.Load<BuildFile>(project.FilePath);
                var distrib = file.Document.GetFirst("DISTRIB_PRIVATE_INC_PATH");
                if (distrib != null)
                {
                    distrib.TryRemove();

                    file.Document.GetAll(Tags.PropertyGroup).ToList()
                                 .ForEach(group => group.RemoveIfEmpty());

                    file.Document.GetAll(Tags.ItemGroup).ToList()
                                 .ForEach(group => group.RemoveIfEmpty());

                    file.Save(omniDeclaration: true);
                }

            }
        }

        public static void RemovePlatForma()
        {
            var produced = Table.GetProducedProjects();
            foreach (var project in produced)
            {
                if (!Path.GetExtension(project.FilePath).EqualsIgnoreCase(".csproj")) continue;

                var file = Repo.Load<BuildFile>(project.FilePath);
                var platform = file.Document.GetFirst(Tags.PlatformTarget);
                if (platform != null)
                {
                    platform.TryRemove();

                    file.Document.GetAll(Tags.PropertyGroup).ToList()
                                 .ForEach(group => group.RemoveIfEmpty());

                    file.Document.GetAll(Tags.ItemGroup).ToList()
                                 .ForEach(group => group.RemoveIfEmpty());

                    file.Save(omniDeclaration: true);
                }

            }
        }
    }
}

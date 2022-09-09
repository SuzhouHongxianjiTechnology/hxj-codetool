namespace Mint.Substrate.LookupTables
{
    using System.Collections.Generic;
    using System.Linq;
    using Mint.Substrate.Configurations;
    using Mint.Substrate.Construction;

    internal class ProjectLookupTable
    {
        private SubstrateProjects repoProjs;

        private ProjectSet? producedProjects;

        private HashSet<string>? cppProjects;

        private HashSet<string>? substrateNuspecs;

        internal ProjectSet ProducedProjects
        {
            get
            {
                if (this.producedProjects == null)
                {
                    var entryProjects = Repo.RestoreEntry.GetProjects(Repo.Paths.SrcDir, new ProjectResolver());
                    this.producedProjects = new ProjectSet(entryProjects.Where(p => p.IsProduced));
                }
                return this.producedProjects;
            }
        }

        internal HashSet<string> CppProjects
        {
            get
            {
                if (this.cppProjects == null)
                {
                    this.cppProjects = this.repoProjs.CppProjects;
                }
                return this.cppProjects;
            }
        }

        public HashSet<string> SubstrateNuspecs
        {
            get
            {
                if (this.substrateNuspecs == null)
                {
                    this.substrateNuspecs = this.repoProjs.SubstrateNuspecs;
                }
                return this.substrateNuspecs;
            }
        }

        internal ProjectLookupTable(SubstrateProjects repoProjs)
        {
            this.repoProjs = repoProjs;
        }
    }
}

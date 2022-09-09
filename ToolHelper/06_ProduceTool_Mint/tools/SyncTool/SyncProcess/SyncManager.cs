namespace SyncTool
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Mint.Common.Utilities;
    using Mint.Substrate;
    using Mint.Substrate.Construction;
    using Mint.Substrate.Porting;
    using Mint.Substrate.Utilities;

    internal class SyncManager
    {
        private LookupTable lookupTable;

        private readonly Git git;

        private List<SyncJob> syncJobs;

        public SyncManager(LookupTable lookupTable)
        {
            this.lookupTable = lookupTable;
            this.git = new Git(AppSettings.DFSrc);
            this.syncJobs = new List<SyncJob>();
        }

        internal void ScanProducedProjects()
        {
            this.git.Switch(AppSettings.FixBranch);

            var restoreEntry = Repo.RestoreEntry;
            restoreEntry.OrganizeProjects();
            restoreEntry.Save();

            foreach (var project in this.lookupTable.GetProducedProjects())
            {
                string producedPath = project.FilePath;
                string nfBuildFilePath = MSBuildUtils.InferNFBuildFileByPath(producedPath);
                if (File.Exists(nfBuildFilePath))
                {
                    var framework = MSBuildUtils.InferFrameworkByPath(producedPath);
                    if (framework == Frameworks.NetFramework)
                    {
                        ConsoleLog.Error(
                            "Project is not named strictly according to the specification." +
                            $"\n(File {producedPath})"
                        );
                        continue;
                    }
                    this.syncJobs.Add(
                        new SyncJob(sourceFilePath: nfBuildFilePath,
                                    producedPath: producedPath,
                                    producedFile: Repo.Load<BuildFile>(producedPath),
                                    framework: framework)
                    );
                }
                else
                {
                    ConsoleLog.Warning(
                        "The corresponding NetFramework projcet file is missing, has it been removed from master branch?" +
                        $"\n(File {producedPath})"
                    );
                }
            }
        }

        internal void CacheMasterProjects()
        {
            this.git.ResetBranchTo(AppSettings.MainBranch, AppSettings.MainCommitID);
            foreach (var job in this.syncJobs.ToList())
            {
                if (!File.Exists(job.SourceFilePath))
                {
                    ConsoleLog.Error(
                        $"Cannot find file on branch: {AppSettings.MainBranch}" +
                        $"\n(File {job.SourceFilePath})"
                    );
                    this.syncJobs.Remove(job);
                }
                job.MainFile = Repo.Load<BuildFile>(job.SourceFilePath);
            }
        }

        internal void CacheDogFoodProjects()
        {
            this.git.ResetBranchTo(AppSettings.DFBranch, AppSettings.DFCommitID);
            foreach (var job in this.syncJobs.ToList())
            {
                if (!File.Exists(job.SourceFilePath))
                {
                    ConsoleLog.Error(
                        $"Cannot find file on branch: {AppSettings.DFBranch}" +
                        $"\n(File {job.SourceFilePath})"
                    );
                    this.syncJobs.Remove(job);
                }
                job.DFFile = Repo.Load<BuildFile>(job.SourceFilePath);
            }
        }

        internal void CompareAndApply()
        {
            var changeList = new List<string>();

            foreach (var job in this.syncJobs)
            {
                var result = job.Execute(this.lookupTable);
                switch (result)
                {
                    case SyncResult.Succeed:
                        ConsoleLog.Success($"[√] {job.ProducedPath}");
                        changeList.Add(job.SourceFilePath);
                        break;

                    case SyncResult.Partially:
                        ConsoleLog.Warning($"[?] {job.ProducedPath}");
                        changeList.Add(job.SourceFilePath);
                        break;

                    case SyncResult.Failed:
                        ConsoleLog.Error($"[×] {job.ProducedPath}");
                        changeList.Add(job.SourceFilePath);
                        break;

                    default:
                        break;
                }
            }
        }

    }
}

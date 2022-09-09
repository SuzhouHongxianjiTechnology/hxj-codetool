namespace Mint.Common.Utilities
{
    public class Git
    {
        private string src;

        public Git(string src)
        {
            this.src = src;
        }

        public void NewBranch(string branch)
        {
            var command = $"git checkout -b {branch}";
            Command.Execute(command, this.src);
        }

        public void Switch(string branch)
        {
            var command = $"git checkout {branch}";
            Command.Execute(command, this.src);
        }

        public void Pull()
        {
            var command = "git pull";
            Command.Execute(command, this.src);
        }

        public void Reset(int step = 0)
        {
            var command = step == 0 ? "git reset --hard head"
                                    : $"git reset --hard head~{step}";
            Command.Execute(command, this.src);
        }

        public void ResetTo(string commitId)
        {
            var command = $"git reset --hard {commitId}";
            Command.Execute(command, this.src);
        }

        public void ResetBranch(string branch)
        {
            Switch(branch);
            Reset();
        }

        public void ResetBranchTo(string branch, string commitId)
        {
            Switch(branch);
            Pull();
            ResetTo(commitId);
        }

        public void Merge(string branch)
        {
            var command = $"git merge {branch}";
            Command.Execute(command, this.src);
        }

        public void MergeRemote(string branch, string remote = "origin")
        {
            var command = $"git merge {remote}/{branch}";
            Command.Execute(command, this.src);
        }
    }
}

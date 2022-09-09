namespace Ceres
{
    using System.IO;
    using Mint.Common.Utilities;

    public class Files
    {
        private static readonly string ceres_data = PathUtils.ApplicationFolder("ceres_data");

        // ------------------------------------------------------------

        public static readonly string moved_imap4 = Path.Combine(ceres_data, "moved_imap4");
        public static readonly string moved_mapi = Path.Combine(ceres_data, "moved_mapi");
        public static readonly string move_plan = Path.Combine(ceres_data, "move_plan");
        public static readonly string ceres_csproj_path = Path.Combine(ceres_data, "ceres_csproj_path");
        public static readonly string project_template = Path.Combine(ceres_data, "project_template");
    }
}

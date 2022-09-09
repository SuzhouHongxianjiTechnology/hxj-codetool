namespace ProcessAnalyser
{
    using System.Collections.Generic;
    using System.IO;
    using Mint.Common.Utilities;
    using Mint.Database;

    public static class Files
    {
        private static readonly string process_data = PathUtils.ApplicationFolder("process_data");

        // ------------------------------------------------------------

        public static readonly string custom_command = Path.Combine(process_data, "custom_command");

        public static readonly Dictionary<Process, string> subCacheMap = new Dictionary<Process, string>
        {
            { Process.Pop3, Path.Combine(process_data, "cache_sub_pop3") } ,
            { Process.Imap4, Path.Combine(process_data, "cache_sub_imap4") } ,
            { Process.MapiHttp, Path.Combine(process_data, "cache_sub_mapi") } ,
            { Process.EAS, Path.Combine(process_data, "cache_sub_eas") },
            { Process.Autodiscover, Path.Combine(process_data, "cache_sub_auto") }
        };

        public static readonly Dictionary<Process, string> nonsubCacheMap = new Dictionary<Process, string>
        {
            { Process.Pop3, Path.Combine(process_data, "cache_nonsub_pop3") } ,
            { Process.Imap4, Path.Combine(process_data, "cache_nonsub_imap4") } ,
            { Process.MapiHttp, Path.Combine(process_data, "cache_nonsub_mapi") } ,
            { Process.EAS, Path.Combine(process_data, "cache_nonsub_eas") },
            { Process.Autodiscover, Path.Combine(process_data, "cache_nonsub_auto") }
        };

    }
}

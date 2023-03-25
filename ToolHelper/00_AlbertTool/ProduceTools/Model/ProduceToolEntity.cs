using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Albert.Model
{
    public class ProduceToolEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public Repo Repo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public MsBuild MsBuild { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public AzureDevOps AzureDevOps { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public PersonalCrawling PersonalCrawling { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public SerilogConfig SerilogConfig { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public HelperInfo HelperInfo { get; set; }

        public BagetRule BagetRule { get; set; }

        public SqlServer SqlServer { get; set; }
        public RedisServer RedisServer { get; set; }
    }

    public class Repo
    {
        /// <summary>
        /// 
        /// </summary>
        public string DefaultPath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CompanyToolEnlistmentPath { get; set; }
    }

    public class MsBuild
    {
        /// <summary>
        /// 
        /// </summary>
        public string Path { get; set; }
    }

    public class AzureDevOps
    {
        /// <summary>
        /// 
        /// </summary>
        public string AzureDevOpsOrganizationUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PersonalAccessToken { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ArtifactIDTemplate { get; set; }
    }

    public class PersonalCrawling
    {
        /// <summary>
        /// 
        /// </summary>
        public string PersonalCrawlingSite { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Expect100Continue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool UseNagleAlgorithm { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool AllowWriteStreamBuffering { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AcceptEncoding { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool AllowAutoRedirect { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UserAgent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Timeout { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool KeepAlive { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SiteJsonStoragePath { get; set; }
    }

    public class SerilogConfig
    {
        public string ExceptionlessClientDefaultStartUpKey { get; set; }
        public string SerilogFilePath { get; set; }
    }

    public class HelperInfo
    {
        public string CmdInformation { get; set; }
        public string RemoteRepoAddress { get; set; }
    }

    public class BagetRule
    {
        public string NugetWebUrl { get; set; }
        public string NugetKey { get; set; }
        public string DelteBagUrl { get; set; }
        public string SearchBagUrl { get; set; }
    }

    public class SqlServer
    {
        public string ConnectStr { get; set; }
    }

    public class RedisServer
    {
        public string Configuration { get; set; }
        public string InstanceName { get; set; }
    }
}


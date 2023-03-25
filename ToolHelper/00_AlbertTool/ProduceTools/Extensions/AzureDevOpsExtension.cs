using Albert.Interface;
using Albert.Model;
using Microsoft.Extensions.Options;
using Microsoft.TeamFoundation.Policy.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Linq;

namespace Albert.Extensions
{
    class AzureDevOpsExtension: IAzure
    {
        private readonly IOptionsSnapshot<ProduceToolEntity> options;
        private static string azureDevOpsOrganizationUrl = "https://dev.azure.com/O365Exchange";
        public  static string artifactIDTemplate = "vstfs:///CodeReview/CodeReviewId/{0}/{1}";

        public AzureDevOpsExtension(IOptionsSnapshot<ProduceToolEntity> options)
        {
            this.options = options;
            if (options != null)
            {
                AzureDevOpsExtension.azureDevOpsOrganizationUrl = options.Value.Repo.DefaultPath;
            }            
        }

        /// <summary>
        /// <see href="AzureAPI访问链接：https://docs.microsoft.com/en-us/rest/api/azure/devops/wit/work-items/list?view=azure-devops-rest-6.1"/>
        /// </summary>
        /// <param name="personalaccesstoken"></param>
        /// <returns></returns>
        public static string GetDFPackageVersionFromCsoProps(string personalaccesstoken)
        {
            try
            {
                string url = "https://dev.azure.com/o365exchange/O365%20Core/_apis/sourceProviders/TfsGit/filecontents?repository=Cso&commitOrBranch=master&path=Packages.props&api-version=6.1-preview.1";

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(
                        string.Format("{0}:{1}", "", personalaccesstoken))));
                    using (HttpResponseMessage responseMessage = client.GetAsync(url).Result)
                    {
                        responseMessage.EnsureSuccessStatusCode();
                        string responseString = responseMessage.Content.ReadAsStringAsync().Result;
                        XDocument xml = XDocument.Load(new StringReader(responseString));
                        string version = xml.Root.Elements()
                            .Where(x => x.Name.LocalName == "ItemGroup").Elements()
                            .Where(x => x.Name.LocalName == "PackageReference")
                            .FirstOrDefault(x => x.Attribute("Update").Value.Equals("Microsoft.Exchange.MapiAbstraction", StringComparison.OrdinalIgnoreCase)).Attribute("Version").Value;
                        version = Regex.Match(version, @"\[(.*)\]").Groups[1].Value;
                        return version;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public static string GetPackageVersionFromFeed(string feedId, string packageName, string personalaccesstoken)
        {
            try
            {
                string url = string.Format(@"https://feeds.dev.azure.com/O365Exchange/O365%20Core/_apis/packaging/Feeds/{0}/packages?packageNameQuery={1}", feedId, packageName);

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(
                        string.Format("{0}:{1}", "", personalaccesstoken))));
                    using (HttpResponseMessage responseMessage = client.GetAsync(url).Result)
                    {
                        responseMessage.EnsureSuccessStatusCode();
                        string responseString = responseMessage.Content.ReadAsStringAsync().Result;
                        var response = JsonConvert.DeserializeObject<Packages>(responseString);
                        if (response.Count == 0) throw new Exception($"Package {packageName} not found.");
                        return response.Value.FirstOrDefault().Versions.FirstOrDefault().normalizedVersion;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
        public static string CreatePullRequest(string repoName, string branchName, string title, string description, string personalaccesstoken)
        {
            VssConnection connection = new VssConnection(new Uri(azureDevOpsOrganizationUrl), new Microsoft.VisualStudio.Services.Common.VssBasicCredential(string.Empty, personalaccesstoken));
            var gitClient = connection.GetClient<GitHttpClient>();
            var gitRepo = gitClient.GetRepositoryAsync("O365 Core", repoName).Result;
            
            var pullRequest = gitClient.CreatePullRequestAsync(
                new GitPullRequest()
                {
                    SourceRefName = "refs/heads/" + branchName,
                    TargetRefName = gitRepo.DefaultBranch,
                    Title = title,
                    Description = description,

                }, gitRepo.Id).Result;
            return $"{gitRepo.WebUrl}/pullrequest/{pullRequest.PullRequestId}";
        }
        public static void SetAutoCompleteWhenPolicysSuccess(string repoName, string prUrl, int timeout, BuildConfig config)
        {
            VssConnection connection = new VssConnection(new Uri(azureDevOpsOrganizationUrl), new Microsoft.VisualStudio.Services.Common.VssBasicCredential(string.Empty, config.PersonalAccessToken));
            var policyClient = connection.GetClient<PolicyHttpClient>();
            var gitClient = connection.GetClient<GitHttpClient>();
            int prId = int.Parse(prUrl.Substring(prUrl.LastIndexOf(@"/") + 1));
            var pr = gitClient.GetPullRequestByIdAsync(prId).Result;
            string artifactId = string.Format(artifactIDTemplate, pr.Repository.ProjectReference.Id, prId);
            List<PolicyEvaluationRecord> policyEvaluationRecords = policyClient.GetPolicyEvaluationsAsync("O365 Core", artifactId).Result;
            int completedPolicyCount = 0;
            foreach(PolicyEvaluationRecord policyEvaluationRecord in policyEvaluationRecords)
            {
                if(config.CsoConfig.RequiredPolicies.ContainsKey(policyEvaluationRecord.Configuration.Id.ToString()))
                {
                    if (policyEvaluationRecord.Status == PolicyEvaluationStatus.Approved) completedPolicyCount++;
                    if (policyEvaluationRecord.Status == PolicyEvaluationStatus.Broken) throw new Exception("Pull Request broken");
                }
            }
            if(config.CsoConfig.RequiredPolicies.Count() == completedPolicyCount)
            {
                var updatePullRequest = new GitPullRequest
                {
                    CompletionOptions = new GitPullRequestCompletionOptions
                    {
                        DeleteSourceBranch = true,
                        MergeCommitMessage = pr.Description,
                        SquashMerge = true
                    },
                    AutoCompleteSetBy = new IdentityRef()
                    {
                        Id = pr.CreatedBy.Id
                    }
                };
                gitClient.UpdatePullRequestAsync(updatePullRequest, "O365 Core", repoName, prId);
            }
            else
            {
                if (timeout == 0) throw new Exception("Required Jobs not finished");
                Thread.Sleep(new TimeSpan(0, timeout, 0));
                SetAutoCompleteWhenPolicysSuccess(repoName, prUrl, 0, config);
            }
        }
        public static void RequeueRequiredPolicy(string prUrl, BuildConfig config)
        {
            VssConnection connection = new VssConnection(new Uri(azureDevOpsOrganizationUrl), new Microsoft.VisualStudio.Services.Common.VssBasicCredential(string.Empty, config.PersonalAccessToken));
            var policyClient = connection.GetClient<PolicyHttpClient>();
            var gitClient = connection.GetClient<GitHttpClient>();
            int prId = int.Parse(prUrl.Substring(prUrl.LastIndexOf(@"/") + 1));
            var pr = gitClient.GetPullRequestByIdAsync(prId).Result;
            string artifactId = string.Format(artifactIDTemplate, pr.Repository.ProjectReference.Id, prId);
            List<PolicyEvaluationRecord> policyEvaluationRecords = policyClient.GetPolicyEvaluationsAsync("O365 Core", artifactId).Result;

            foreach (PolicyEvaluationRecord policyEvaluationRecord in policyEvaluationRecords)
            {
                if(config.Pop3Config.RequiredPolicies.ContainsKey(policyEvaluationRecord.Configuration.Id.ToString()) && policyEvaluationRecord.Status == PolicyEvaluationStatus.Queued)
                {
                    policyClient.RequeuePolicyEvaluationAsync("O365 Core", policyEvaluationRecord.EvaluationId);
                }
            }
        }

        public static bool PRIsCompleted(string prUrl, BuildConfig config)
        {
            VssConnection connection = new VssConnection(new Uri(azureDevOpsOrganizationUrl), new Microsoft.VisualStudio.Services.Common.VssBasicCredential(string.Empty, config.PersonalAccessToken));
            var gitClient = connection.GetClient<GitHttpClient>();
            int prId = int.Parse(prUrl.Substring(prUrl.LastIndexOf(@"/") + 1));
            var pr = gitClient.GetPullRequestByIdAsync(prId).Result;
            return pr.Status == PullRequestStatus.Completed;
        }

        public void RunAzureDevOpsExtension(IServiceProvider sp, string[] args)
        {

        }
    }
}

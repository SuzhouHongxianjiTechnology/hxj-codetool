
namespace Scripts
{
    using System.Collections.Generic;
    using System.IO;
    using Mint.Common.Extensions;
    using Mint.Common.Utilities;
    using Mint.Substrate;
    using Mint.Substrate.Construction;

    public class Lazyness
    {
        static HashSet<string> mapiCeresDlls = new HashSet<string>() {
            "Microsoft.Exchange.Compliance.TaskDistributionCommon.dll",
            "Microsoft.Exchange.ContentProcessing.Provider.dll",
            "Microsoft.Exchange.ContentProcessing.WordBreaker.dll",
            "Microsoft.Exchange.Data.GroupMailboxAccessLayer.dll",
            "Microsoft.Exchange.Data.Search.Common.dll",
            "Microsoft.Exchange.Data.Search.dll",
            "Microsoft.Exchange.Hygiene.DataInsights.Kql.dll",
            "Microsoft.Exchange.InfoWorker.Common.dll",
            "Microsoft.Exchange.Lss.dll",
            "Microsoft.Exchange.Management.dll",
            "Microsoft.Exchange.Query.Analysis.dll",
            "Microsoft.Exchange.Query.Core.dll",
            "Microsoft.Exchange.Query.Ranking.dll",
            "Microsoft.Exchange.Query.Retrieval.dll",
            "Microsoft.Exchange.Query.Suggestions.dll",
            "Microsoft.Exchange.Search.BigFunnel.dll",
            "Microsoft.Exchange.Search.ContentProcessing.dll",
            "Microsoft.Exchange.Search.Engine.dll",
            "Microsoft.Exchange.Search.Fast.Ceres.dll",
            "Microsoft.Exchange.Search.Fast.dll",
            "Microsoft.Exchange.Search.Files.dll",
            "Microsoft.Exchange.Search.LingOperators.dll",
            "Microsoft.Exchange.Search.Mailbox.dll",
            "Microsoft.Exchange.Search.MailboxOperators.dll",
            "Microsoft.Exchange.Search.Mdb.Extensions.dll",
            "Microsoft.Exchange.Search.Mdb.dll",
            "Microsoft.Exchange.Search.OperatorSchema.IO.dll",
            "Microsoft.Exchange.Search.OperatorSchema.dll",
            "Microsoft.Exchange.Search.Token.dll",
            "Microsoft.Exchange.Search.TokenOperators.dll",
            "Microsoft.Exchange.Services.dll"
        };

        static HashSet<string> mapiCeresProjects = new HashSet<string>() {
            @"sources\dev\EDiscovery\src\TaskDistributionSystem\TaskDistributionCommon\Microsoft.Exchange.Compliance.TaskDistributionCommon.csproj",
            @"sources\dev\ContentProcessing\src\Provider\Microsoft.Exchange.ContentProcessing.Provider.csproj",
            @"sources\dev\ContentProcessing\src\WordBreaker\Microsoft.Exchange.ContentProcessing.WordBreaker.csproj",
            @"sources\dev\UnifiedGroups\src\UnifiedGroups\Microsoft.Exchange.Data.GroupMailboxAccessLayer.csproj",
            @"sources\dev\data\src\Search.Common\Microsoft.Exchange.Data.Search.Common.csproj",
            @"sources\dev\data\src\Search\Microsoft.Exchange.Data.Search.csproj",
            @"sources\dev\Hygiene\src\DataInsights\Kql\Microsoft.Exchange.Hygiene.DataInsights.Kql.csproj",
            @"sources\dev\infoworker\src\common\Server\Microsoft.Exchange.InfoWorker.Common.csproj",
            @"sources\dev\services\src\Lss\Microsoft.Exchange.Lss.csproj",
            @"sources\dev\Management\src\Management\base\Microsoft.Exchange.Management.csproj",
            @"sources\dev\Query\src\Analysis\Microsoft.Exchange.Query.Analysis.csproj",
            @"sources\dev\Query\src\Core\Microsoft.Exchange.Query.Core.csproj",
            @"sources\dev\Query\src\Ranking\Microsoft.Exchange.Query.Ranking.csproj",
            @"sources\dev\Query\src\Retrieval\Microsoft.Exchange.Query.Retrieval.csproj",
            @"sources\dev\Query\src\Suggestions\Microsoft.Exchange.Query.Suggestions.csproj",
            @"sources\dev\Search\src\BigFunnel\Microsoft.Exchange.Search.BigFunnel.csproj",
            @"sources\dev\Search\src\ContentProcessing\Microsoft.Exchange.Search.ContentProcessing.csproj",
            @"sources\dev\Search\src\Engine\Microsoft.Exchange.Search.Engine.csproj",
            @"sources\dev\Search\src\Fast.Ceres\Microsoft.Exchange.Search.Fast.Ceres.csproj",
            @"sources\dev\Search\src\Fast\Microsoft.Exchange.Search.Fast.csproj",
            @"sources\dev\Search\src\Files\Microsoft.Exchange.Search.Files.csproj",
            @"sources\dev\Search\src\LingOperators\Microsoft.Exchange.Search.LingOperators.csproj",
            @"sources\dev\Search\src\Mailbox\Microsoft.Exchange.Search.Mailbox.csproj",
            @"sources\dev\Search\src\MailboxOperators\Microsoft.Exchange.Search.MailboxOperators.csproj",
            @"sources\dev\Search\src\Mdb.Extensions\Microsoft.Exchange.Search.Mdb.Extensions.csproj",
            @"sources\dev\Search\src\Mdb\Microsoft.Exchange.Search.Mdb.csproj",
            @"sources\dev\Search\src\OperatorSchemaIO\Microsoft.Exchange.Search.OperatorSchemaIO.csproj",
            @"sources\dev\Search\src\OperatorSchema\Microsoft.Exchange.Search.OperatorSchema.csproj",
            @"sources\dev\Search\src\Token\Microsoft.Exchange.Search.Token.csproj",
            @"sources\dev\Search\src\TokenOperators\Microsoft.Exchange.Search.TokenOperators.csproj",
            @"sources\dev\services\src\Services\Microsoft.Exchange.Services.csproj",
        };

        public static void Foo()
        {
            // foreach (var assembly in mapiCeresDlls)
            // {
            //     var detail = AssemblyDetails.RequestAsync(assembly, "15.20.4263.000", Process.MapiHttp).Result;
            //     ConsoleLog.Highlight(detail.SourcePath);
            // }

            var table = new LookupTable();
            int count = 0;
            foreach (var relativePath in mapiCeresProjects)
            {
                var fullPath = Path.Combine(Repo.Paths.SrcDir, relativePath);
                var file = Repo.Load<BuildFile>(fullPath);
                var refResolver = new ReferenceResolver(fullPath, table);
                var refes = file.GetReferences(refResolver);

                foreach (var r in refes)
                {
                    if (r.Type == ReferenceType.NuGet && r.ReferenceName.EqualsIgnoreCase("Microsoft.Fast.Search"))
                    {
                        count ++;
                        ConsoleLog.Warning(file.AssemblyName);
                        ConsoleLog.Path(fullPath);
                        ConsoleLog.Success($"    {r.ReferenceDll}");
                        ConsoleLog.Ignore("");
                    }
                }

            }
            ConsoleLog.Message(count);
        }
    }
}

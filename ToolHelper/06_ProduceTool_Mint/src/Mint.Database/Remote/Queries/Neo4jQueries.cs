namespace Mint.Database.Remote
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Mint.Database.Configurations;
    using Neo4jClient;
    using Neo4jClient.Cypher;

    public static class Neo4jQueries
    {
        private static DatabaseSettings settings = DatabaseSettings.Settings;
        private static Uri Neo4jUri = new Uri(settings.Neo4j_Host);

        public static async Task<HashSet<string>> GetCeresTypesByProcessAsync(string version, string process)
        {
            using (var client = new BoltGraphClient(Neo4jUri, username: settings.Neo4j_User, password: settings.Neo4j_Pass))
            {
                client.ConnectAsync().Wait();

                /*
                MATCH (ProcessNode{version:'15.20.4263.000',processName:'MapiHttp'})-[r:ProcessContainsType]->(t:TypeNode{version:'15.20.4263.000'})
                WHERE r.cost > 0 AND t.typeKey STARTS WITH 'Microsoft.Ceres'
                RETURN t.typeKey
                */

                var query = client.Cypher
                    .OptionalMatch($"(ProcessNode{{version:'{version}',processName:'{process}'}})-[r:ProcessContainsType]->(t:TypeNode{{version:'{version}'}})")
                    .Where("r.cost > 0")
                    .AndWhere("t.typeKey STARTS WITH 'Microsoft.Ceres'")
                    .WithParam("version", version)
                    .Return<string>("t.typeKey");

                return (await query.ResultsAsync).ToHashSet();
            }
        }

        public static async Task<List<List<string>>> GetProcessToTypePathsAsync(string process, string version, string tarType)
        {
            using (var client = new BoltGraphClient(Neo4jUri, username: settings.Neo4j_User, password: settings.Neo4j_Pass))
            {
                client.ConnectAsync().Wait();

                var nodeQuery = @"'
                    MATCH (a:ProcessNode{processName: $process, version: $version})
                    RETURN id(a) AS id
                    UNION ALL
                    MATCH (p:ProcessNode{processName: $process, version: $version})-[r:ProcessContainsAssembly]->(b:AssemblyNode{version: $version})
                    WHERE r.abandon IS NULL
                    RETURN id(b) AS id
                    UNION ALL
                    MATCH (p:ProcessNode{processName: $process, version: $version})-[r:ProcessContainsAssembly]->(a:AssemblyNode{version: $version})-[def:AssemblyDef]->(t:TypeNode{version: $version})
                    WHERE r.abandon IS NULL
                    RETURN id(t) AS id
                '";

                var relationshipQuery = @"'
                    MATCH (a:ProcessNode{processName: $process, version: $version})-[r:StartAssembly]->(b)
                    RETURN id(a) AS source, id(b) AS target
                    UNION ALL
                    MATCH (a:ProcessNode{processName: $process, version: $version})-[r:StartAssembly]->(b)
                    WITH b
                    MATCH (b)-[r:AssemblyDef]->(c)
                    RETURN id(b) AS source, id(c) AS target
                    UNION ALL
                    MATCH (p:ProcessNode{processName: $process, version: $version})-[r:ProcessContainsAssembly]->(a)
                    WHERE r.abandon IS NULL
                    WITH a
                    Match (a)-[:AssemblyDef]->(t)
                    WITH collect(id(t)) as restriction
                    MATCH (t1:TypeNode{version: $version})-[r:TypeRef]->(t2:TypeNode{version: $version})
                    WHERE id(t1) in restriction AND id(t2) in restriction AND (NOT t1.typeKey =~ \'(.*ServiceModel)|(.*Command)\') AND (NOT t2.typeKey =~ \'(.*ServiceModel)|(.*Command)\')
                    RETURN id(t1) AS source, id(t2) AS target
                '";

                var query = client.Cypher
                   .Match("(start:ProcessNode{processName: $process, version: $version}), (end:TypeNode{typeName: $tarType, version: $version})")
                   .Call(string.Format(@"gds.alpha.kShortestPaths.stream({{
                                            startNode: start,
                                            endNode: end,
                                            k: $k,
                                            nodeQuery: {0},
                                            relationshipQuery: {1},
                                            parameters: {{
                                                process: $process,
                                                version: $version
                                            }}
                                        }})", nodeQuery, relationshipQuery))
                   .WithParam("process", process)
                   .WithParam("version", version)
                   .WithParam("tarType", tarType)
                   .WithParam("k", 10)
                   .Yield("nodeIds")
                   .Return(() => Return.As<List<string>>("[node in gds.util.asNodes(nodeIds) | case when 'ProcessNode' in Labels(node) then node.processName when 'AssemblyNode' in Labels(node) then node.assemblyName else node.typeName end] AS names"));

                return (List<List<string>>) await query.ResultsAsync;
            }
        }
    }
}

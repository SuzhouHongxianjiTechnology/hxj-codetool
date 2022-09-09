namespace Mint.Database.Local
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Mint.Common.Extensions;
    using Mint.Database.Configurations;
    using Mint.DataStructures.DirectedGraph;
    using Newtonsoft.Json;

    public static class AllTypes
    {
        public static Graph<string, int> BuildGraphWithCondition(Func<TypeNode, bool> condition)
        {
            var graph = new Graph<string, int>();
            var types = AllTypes.Load().Filter(condition);
            foreach (var type in types)
            {
                type.Children.Filter(condition)
                             .ForEach(child => graph.AddEdge(type.TypeKey, child.TypeKey, 0));
            }
            return graph;
        }

        public static List<TypeNode> Load()
        {
            var defaultSource = DatabaseSettings.Settings.AllTypesJson;
            // return FileUtils.DeserializeJsonAsync<Dictionary<string, TypeNode>>(defaultSource).Result;

            using (StreamReader json = File.OpenText(defaultSource))
            {
                var serializer = JsonSerializer.Create(
                    new JsonSerializerSettings { MaxDepth = null, MissingMemberHandling = MissingMemberHandling.Ignore }
                );
                var serialized = serializer.Deserialize(json, typeof(Dictionary<string, TypeNode>));
                if (serialized != null)
                {
                    return ((Dictionary<string, TypeNode>) serialized).Values.ToList();
                }
            }
            throw new Exception("Serialize AllTypes.json failed.");
        }
    }
}

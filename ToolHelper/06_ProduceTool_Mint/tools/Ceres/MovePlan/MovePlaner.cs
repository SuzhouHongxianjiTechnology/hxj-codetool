namespace Ceres
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Text.RegularExpressions;
    using Mint.Common.Utilities;
    using Mint.Database.Local;
    using Mint.Database.Remote;
    using Mint.DataStructures.DirectedGraph;

    public class MovePlaner
    {
        public string Version { get; }

        private Graph<string, int> typeGraph { get; set; }
        private Graph<string, int> asseGraph { get; set; }

        private HashSet<string>? movedCeresTypes;

        public MovePlaner(string version)
        {
            this.Version = version;
            this.typeGraph = new Graph<string, int>();
            this.asseGraph = new Graph<string, int>();
        }

        internal void BuildGraph(string process, bool ignoreMoved = false)
        {

            var processCeresType = Neo4jQueries.GetCeresTypesByProcessAsync(this.Version, process).Result;

            ConsoleLog.Ignore("");
            ConsoleLog.InLine(" * Querying Ceres types ...... ");

            var allMoved = new HashSet<string>();

            var imapCeresTypes = Neo4jQueries.GetCeresTypesByProcessAsync(this.Version, "Imap4").Result;
            var mapiCeresTypes = Neo4jQueries.GetCeresTypesByProcessAsync(this.Version, "MapiHttp").Result;
            allMoved.UnionWith(imapCeresTypes);
            allMoved.UnionWith(mapiCeresTypes);

            // var movedImap = FileUtils.ReadText(Files.moved_imap4).ToHashSet();
            // var movedMapi = FileUtils.ReadText(Files.moved_mapi).ToHashSet();
            // allMoved.UnionWith(movedImap);
            // allMoved.UnionWith(movedMapi);

            this.movedCeresTypes = ignoreMoved ? new HashSet<string>() : allMoved;

            int needCount = processCeresType.Count;
            int moveCount = this.movedCeresTypes.Count;
            ConsoleLog.Ignore($"Needed:{needCount}, Moved:{moveCount}");

            ConsoleLog.InLine(" * Building type graph ....... ");
            var typesNeedMove = ignoreMoved ? processCeresType : processCeresType.Except(this.movedCeresTypes).ToHashSet();

            FileUtils.WriteText(Files.moved_mapi, typesNeedMove);
            this.typeGraph = AllTypes.BuildGraphWithCondition(t => typesNeedMove.Contains(t.TypeKey));
            this.typeGraph.ClearSelfCycle();
            ConsoleLog.Ignore($"Graph: {this.typeGraph.VerticeCount} vertices, {this.typeGraph.EdgeCount} edges");

            ConsoleLog.InLine(" * Building assembly graph ... ");
            foreach (var edge in this.typeGraph.Edges)
            {
                string srcAsse = edge.Source.Split('.')[2];
                string dstAsse = edge.Destination.Split('.')[2];
                this.asseGraph.AddEdge(srcAsse, dstAsse, 0);
            }
            this.asseGraph.ClearSelfCycle();
            ConsoleLog.Ignore($"Graph: {this.asseGraph.VerticeCount} vertices, {this.asseGraph.EdgeCount} edges");
        }

        internal List<WaveItem> GenerateMovePlan()
        {
            var waveItems = new List<WaveItem>();
            var moveWaves = this.asseGraph.TopologicalWave(out List<Vertice<string>> remains);

            var pattern = new Regex(@"([^\w,.].*)(Microsoft)|([^\w,.].*)");

            int totalTypes = 0;
            for (int i = 0; i < moveWaves.Count; i++)
            {
                var wave = moveWaves[i];
                var types = new List<string>();
                foreach (var assembly in wave)
                {
                    foreach (var vertice in this.typeGraph.Vertices)
                    {
                        if (vertice.Value.Split(',')[1].StartsWith('<'))
                        {
                            continue;
                        }
                        if (vertice.Value.Split('.')[2].Equals(assembly))
                        {
                            string cleanVal = pattern.Replace(vertice.Value, "$2");
                            types.Add(cleanVal);
                        }
                    }
                }
                types.Sort();
                types = types.Distinct().ToList();
                waveItems.Add(new WaveItem(i + 1, types));
                totalTypes += types.Count;
                ConsoleLog.Message($" * Wave: {i + 1}, Assemblies: {wave.Count}, Types: {types.Count}");
            }
            ConsoleLog.Message($" * Total Types: {totalTypes}");
            if (remains.Any())
            {
                ConsoleLog.Error($" * Cycle ref detected. There are {remains.Count} assemblies left in the graph.");
                // foreach (var vertice in remains)
                // {
                //     ConsoleLog.Error($"   - Microsoft.Ceres.{vertice.Value}.*");
                //     vertice.Children.ForEach(c => ConsoleLog.Error($"     - Microsoft.Ceres.{c.Value}.*"));
                // }
            }
            return waveItems;
        }

        internal void DisplayRelationship()
        {
            foreach (var vertice in this.asseGraph.Vertices)
            {
                ConsoleLog.Warning($"\n   Microsoft.Ceres.{vertice.Value}");
                foreach (var edge in this.asseGraph.Edges)
                {
                    if (edge.Source == vertice.Value)
                    {
                        ConsoleLog.Highlight($"    > Microsoft.Ceres.{edge.Destination}");
                    }
                }
            }
        }

        public void SaveToJson(List<WaveItem> waveItems)
        {
            var options = new JsonSerializerOptions() { WriteIndented = true };
            string json = JsonSerializer.Serialize<List<WaveItem>>(waveItems, options);
            FileUtils.WriteJson(Files.move_plan, json);
        }

        internal void ProcessUsedCeresTypes(string process)
        {
            var mcTypes = Neo4jQueries.GetCeresTypesByProcessAsync(this.Version, process).Result;
            FileUtils.CreateAndWriteLines(@"D:\mapi_ceres_types.txt", mcTypes.ToList());
            var graph = AllTypes.BuildGraphWithCondition(t => mcTypes.Contains(t.TypeKey));
            ConsoleLog.Debug($"All {process} Ceres Types: {mcTypes.Count}");
            ConsoleLog.Debug($"All Related Ceres Types: {graph.VerticeCount}");
        }
    }
}

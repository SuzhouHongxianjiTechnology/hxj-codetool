
namespace Mint.DataStructures.DirectedGraph
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class Algorithms
    {

        private static void DFS_TopDown<V, E>(this Graph<V, E> graph, Vertice<V> root, Action<Vertice<V>>? action = null) where V : IEquatable<V>
        {
            graph.DepthFirstSearch(root, topdown: true, action);
        }

        private static void DFS_ButtomUp<V, E>(this Graph<V, E> graph, Vertice<V> root, Action<Vertice<V>>? action = null) where V : IEquatable<V>
        {
            graph.DepthFirstSearch(root, topdown: false, action);
        }

        private static void DepthFirstSearch<V, E>(this Graph<V, E> graph, Vertice<V> root, bool topdown = false, Action<Vertice<V>>? action = null) where V : IEquatable<V>
        {
            if (!root.IsVisited)
            {
                root.IsVisited = true;
                var next = topdown ? root.Children : root.Parents;
                foreach (var vertice in next)
                {
                    graph.DepthFirstSearch(vertice, topdown, action);
                }
                if (action != null) action(root);
            }
        }

        public static Stack<Vertice<V>> TopologicalSort<V, E>(this Graph<V, E> graph) where V : IEquatable<V>
        {
            graph.ResetVisited();
            var stack = new Stack<Vertice<V>>();
            foreach (var vertice in graph.Vertices)
            {
                graph.DFS_TopDown(vertice, n => stack.Push(n));
            }
            return stack;
        }

        public static List<HashSet<V>> TopologicalWave<V, E>(this Graph<V, E> graph, out List<Vertice<V>> remains) where V : IEquatable<V>
        {
            remains = new List<Vertice<V>>();

            var clone = graph.Clone();
            var waves = new List<HashSet<V>>();

            while (clone.VerticeCount > 0)
            {
                var wave = new HashSet<V>();
                foreach (var vertice in clone.Vertices)
                {
                    if (vertice.OutDegree == 0)
                    {
                        wave.Add(vertice.Value);
                    }
                }
                if (!wave.Any())
                {
                    remains = clone.Vertices;
                    break;
                }
                else
                {
                    foreach (var vertice in wave)
                    {
                        clone.RemoveVertice(vertice);
                    }
                }
                waves.Add(wave);
            }
            return waves;
        }

        public static List<List<Vertice<V>>> StronglyConnectedComponents<V, E>(this Graph<V, E> graph) where V : IEquatable<V>
        {
            var scc = new List<List<Vertice<V>>>();
            var stack = graph.TopologicalSort();
            graph.ResetVisited();
            while (stack.Count > 0)
            {
                var vertice = stack.Pop();
                if (!vertice.IsVisited)
                {
                    var component = new List<Vertice<V>>();
                    graph.DFS_ButtomUp(vertice, n => component.Add(n));
                    scc.Add(component);
                }
            }
            return scc;
        }
    }
}

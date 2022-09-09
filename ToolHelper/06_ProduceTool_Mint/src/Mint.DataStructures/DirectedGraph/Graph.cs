namespace Mint.DataStructures.DirectedGraph
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Graph<V, E> where V : IEquatable<V>
    {
        private Dictionary<V, Vertice<V>> vertices { get; }

        private Dictionary<Tuple<V, V>, Edge<V, E>> edges { get; }

        public List<Vertice<V>> Vertices => this.vertices.Values.ToList();

        public List<Edge<V, E>> Edges => this.edges.Values.ToList();

        public int VerticeCount => this.Vertices.Count;

        public int EdgeCount => this.Edges.Count;

        public Graph()
        {
            this.vertices = new Dictionary<V, Vertice<V>>();
            this.edges = new Dictionary<Tuple<V, V>, Edge<V, E>>();
        }

        public Graph<V, E> Clone()
        {
            var newGraph = new Graph<V, E>();
            this.Vertices.ForEach(v => newGraph.AddVertice(v.Clone()));
            this.Edges.ForEach(e => newGraph.AddEdge(e.Clone()));
            return newGraph;
        }

        public void ResetVisited()
        {
            this.Vertices.ForEach(v => v.IsVisited = false);
        }

        public void ClearSelfCycle()
        {
            this.Edges.Where(e => e.Source.Equals(e.Destination)).ToList().ForEach(e => this.RemoveEdge(e));
        }

        #region Vertices

        public bool ContainsVertice(V vertice)
        {
            return this.vertices.ContainsKey(vertice);
        }

        public Vertice<V>? GetVertice(V target)
        {
            this.vertices.TryGetValue(target, out Vertice<V>? vertice);
            return vertice;
        }

        private Vertice<V> GetOrAddVertice(V vertice)
        {
            if (this.vertices.TryGetValue(vertice, out Vertice<V>? target))
            {
                return target;
            }
            var newVertice = new Vertice<V>(vertice);
            this.AddVertice(newVertice);
            return newVertice;
        }

        public void AddVertice(V vertice)
        {
            this.vertices.TryAdd(vertice, new Vertice<V>(vertice));
        }

        private void AddVertice(Vertice<V> vertice)
        {
            this.vertices.TryAdd(vertice.Value, vertice);
        }

        public void RemoveVertice(V vertice)
        {
            if (this.vertices.TryGetValue(vertice, out Vertice<V>? target))
            {
                target.Parents.ToList().ForEach(p => p.Children.Remove(target));
                target.Children.ToList().ForEach(c => c.Parents.Remove(target));
                this.RemoveEdgeFrom(vertice);
                this.RemoveEdgeTo(vertice);
                this.vertices.Remove(vertice);
            }
        }

        public void RemoveVertice(Vertice<V> vertice)
        {
            this.RemoveVertice(vertice.Value);
        }

        #endregion

        #region Edge

        public bool ContainsEdge(V source, V dest)
        {
            return this.edges.ContainsKey(new Tuple<V, V>(source, dest));
        }

        public Edge<V, E>? GetEdge(V source, V dest)
        {
            this.edges.TryGetValue(new Tuple<V, V>(source, dest), out Edge<V, E>? edge);
            return edge;
        }

        public void AddEdge(V source, V dest, E data)
        {
            var parent = this.GetOrAddVertice(source);
            var child = this.GetOrAddVertice(dest);
            parent.Children.Add(child);
            child.Parents.Add(parent);
            this.edges.TryAdd(new Tuple<V, V>(source, dest), new Edge<V, E>(source, dest, data));
        }

        private void AddEdge(Edge<V, E> edge)
        {
            this.AddEdge(edge.Source, edge.Destination, edge.Value);
        }

        public void RemoveEdge(V source, V dest)
        {
            var key = new Tuple<V, V>(source, dest);
            if (this.edges.ContainsKey(key))
            {
                var parent = this.GetOrAddVertice(source);
                var child = this.GetOrAddVertice(dest);
                parent.Children.Remove(child);
                child.Parents.Remove(parent);
                this.edges.Remove(key);
            }
        }

        public void RemoveEdge(Edge<V, E> edge)
        {
            this.RemoveEdge(edge.Source, edge.Destination);
        }

        public void RemoveEdgeFrom(V source)
        {
            if (this.ContainsVertice(source))
            {
                this.GetOrAddVertice(source).Children.ToList().ForEach(c => this.RemoveEdge(source, c.Value));
            }
        }

        public void RemoveEdgeTo(V dest)
        {
            if (this.ContainsVertice(dest))
            {
                this.GetOrAddVertice(dest).Parents.ToList().ForEach(p => this.RemoveEdge(p.Value, dest));
            }
        }

        #endregion
    }
}

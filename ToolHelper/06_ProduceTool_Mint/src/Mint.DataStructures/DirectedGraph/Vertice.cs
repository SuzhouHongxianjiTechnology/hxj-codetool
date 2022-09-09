namespace Mint.DataStructures.DirectedGraph
{
    using System.Collections.Generic;

    public class Vertice<V> where V : notnull
    {
        public V Value { get; }

        public HashSet<Vertice<V>> Parents { get; internal set; }

        public HashSet<Vertice<V>> Children { get; internal set; }

        public int InDegree => this.Parents.Count;

        public int OutDegree => this.Children.Count;

        public bool IsVisited { get; set; }

        public Vertice(V value)
        {
            this.Value = value;
            this.Parents = new HashSet<Vertice<V>>();
            this.Children = new HashSet<Vertice<V>>();
            this.IsVisited = false;
        }

        public Vertice<V> Clone()
        {
            return new Vertice<V>(this.Value);
        }
    }

}

namespace Mint.DataStructures.DirectedGraph
{

    public class Edge<V, E>
    {
        public V Source { get; }

        public V Destination { get; }

        public E Value { get; }

        public Edge(V source, V dest, E value)
        {
            this.Source = source;
            this.Destination = dest;
            this.Value = value;
        }

        public Edge<V, E> Clone()
        {
            return new Edge<V, E>(this.Source, this.Destination, this.Value);
        }
    }
}

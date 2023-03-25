namespace Mint.Substrate.Construction
{
    using System.Collections.Generic;
    using Mint.Substrate.Production;

    public interface IComparator<T> where T : NetFrameworkProjectFile
    {
        List<BaseDiff> CompareTo(T project1, T project2, PortingConfig config);
    }
}

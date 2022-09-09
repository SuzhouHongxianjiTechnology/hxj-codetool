namespace Mint.Substrate.Construction
{
    public interface IProject
    {
        string Name { get; }

        string FilePath { get; }

        ProjectType Type { get; }

        string Framework { get; }

        bool IsProduced { get; }

        string? TargetPath { get; }
    }
}

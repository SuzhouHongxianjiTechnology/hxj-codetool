namespace Mint.Substrate.Construction
{
    using System.IO;
    using Mint.Common.Extensions;
    using Mint.Substrate.Utilities;

    public class ProjectResolver
    {
        public IProject Resolve(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new UnknownProject(filePath);
            }

            var extension = Path.GetExtension(filePath);

            if (extension.EqualsIgnoreCase(".csproj"))
            {
                var file = Repo.Load<BuildFile>(filePath);
                string framework = file.Document.GetFirst(Tags.TargetFramework)?.Value
                                   ?? MSBuildUtils.InferFrameworkByPath(filePath);
                return new Project(file.AssemblyName, framework, filePath, ProjectType.Substrate);
            }
            else
            {
                var file = Repo.Load(filePath);
                string name = file.Document.GetFirst(Tags.AssemblyName)?.Value
                              ?? Path.GetFileNameWithoutExtension(filePath);
                string framework = file.Document.GetFirst(Tags.TargetFramework)?.Value
                                   ?? MSBuildUtils.InferFrameworkByPath(filePath);
                return new Project(name, framework, filePath, ProjectType.CPP);
            }

        }
    }
}

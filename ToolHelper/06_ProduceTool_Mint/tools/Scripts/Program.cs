namespace Scripts
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Mint.Common.Extensions;
    using Mint.Common.Utilities;
    using Mint.Database;
    using Mint.Database.Remote;
    using Mint.Substrate;
    using Mint.Substrate.Construction;

    class Program
    {
        static void Main(string[] args)
        {
            string parent = @"\\sources\dev\common\src\Net\Server\";
            string child = @"..\Server\Logging\*.cs";

            var p1 = Path.Combine(parent, child);
            var p2 = @"\\sources\dev\common\src\Net\Server\..\Server\Logging\*.cs";

            ConsoleLog.Error(Path.GetFullPath(p1));
            ConsoleLog.Warning(Path.GetFullPath(p2));

            // var allProduced = Repo.RestoreEntry.GetProjects(Repo.Paths.SrcDir, new ProjectResolver());
            // foreach (var proj in allProduced)
            // {
            //     var buildFile = Repo.Load<BuildFile>(proj.FilePath);
            //     var target = buildFile.Document.GetFirst(Tags.TargetFramework);
            //     if (target != null)
            //     {
            //         var val = target.Value;
            //         if (val == "netcoreapp3.1" || val == "netstandard2.0" || val == "net5.0")
            //         {
            //             continue;
            //         }
            //         else
            //         {
            //             ConsoleLog.Error(proj.FilePath);
            //         }
            //     }
            //     else
            //     {
            //         // ConsoleLog.Highlight(target?.Value);
            //         ConsoleLog.Error(proj.FilePath);
            //     }
            // }
        }


        private static async Task<string?> GetProduceData(string assembly, string version)
        {
            var detail = await AssemblyDetails.RequestAsync(assembly, version: version, process: Process.MapiHttp);

            if (detail == null)
            {
                return default;
            }

            /*
            "properties":{
                "nugetInfo":null,
                "producedInfo":{
                    "producedState":"True",
                    "producedAuthor":"Jiuchen",
                    "producedComment":null,
                    "version":"15.20.3635.000"}
                }
            }
            */

            var properties = detail.Properties;
            if (properties == null)
            {
                return default;
            }

            var produceInfo = properties.ProducedInfo;
            if (produceInfo == null)
            {
                return default;
            }

            var result = new List<string>();
            result.Add(detail.AssemblyName);
            result.Add(produceInfo.producedState);
            result.Add(produceInfo.ProducedAuthor);
            result.Add(produceInfo.ProducedComment);
            result.Add(produceInfo.Version);
            return string.Join(',', result);
        }
    }
}

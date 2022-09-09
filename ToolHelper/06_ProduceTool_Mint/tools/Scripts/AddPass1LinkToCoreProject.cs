using Mint.Substrate;
using Mint.Substrate.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Scripts
{
    public class AddPass1LinkToCoreProject
    {
        public LookupTable Table = new LookupTable();

        public AddPass1LinkToCoreProject()
        {
        }

        public void Hander()
        {
            var paths = GetPaths();

            Dictionary<string, string> notFindFiles = new Dictionary<string, string>();

            List<string> hasNeedCopyNodeFiles = new List<string>();

            foreach (var item in paths)
            {
                if (!File.Exists(item.Key) || !File.Exists(item.Value))
                {
                    notFindFiles.Add(item.Key, item.Value);
                    continue;
                }

                XmlDocument frameworkXml = new XmlDocument();
                frameworkXml.Load(item.Value);

                var needCopyNodes = frameworkXml.GetElementsByTagName("Pass1Link");

                if (needCopyNodes.Count == 0)
                {
                    continue;
                }

                XmlDocument coreXml = new XmlDocument();
                coreXml.Load(item.Key);

                var ifHas = coreXml.GetElementsByTagName("Pass1Link");
                if (ifHas.Count > 0)
                {
                    continue;
                }

                hasNeedCopyNodeFiles.Add(item.Value);

                XmlElement ele = coreXml.CreateElement("Pass1Link");
                ele.InnerText = needCopyNodes[0].InnerText;

                XmlNode whereInsert = coreXml.GetElementsByTagName("AssemblyName")[0];

                coreXml.GetElementsByTagName("PropertyGroup")[0].InsertAfter(ele, whereInsert);

                coreXml.Save(item.Key);
            }

            var currentDirectory = Directory.GetCurrentDirectory();

            var str1 = new StringBuilder();
            foreach (var item in notFindFiles)
            {
                str1.Append(item.Key + "      " + item.Value + "\\n");
            }
            File.WriteAllText(currentDirectory + "/Pass1Link_notfind.md", str1.ToString());

            string text2 = string.Join("   \n", hasNeedCopyNodeFiles);
            File.WriteAllText(currentDirectory + "/Pass1Link_notfind_needCopy.md", text2);
        }

        public Dictionary<string, string> GetPaths()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            var coreProject = Table.GetProducedProjects();

            foreach (var item in coreProject.AsDictionary().Values)
            {
                string netFxPath = MSBuildUtils.InferNFBuildFileByPath(item.FilePath);
                dic.Add(item.FilePath, netFxPath);
            }
            return dic;
        }
    }
}

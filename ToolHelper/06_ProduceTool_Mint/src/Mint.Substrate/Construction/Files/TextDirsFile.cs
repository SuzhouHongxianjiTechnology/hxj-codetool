namespace Mint.Substrate.Construction
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Mint.Common.Extensions;

    public sealed class TextDirsFile
    {
        private string path;

        private List<string> content;

        public TextDirsFile(string path)
        {
            this.path = path;
            this.content = new List<string>();
        }

        public void RemovePath(string path)
        {
            this.content = new List<string>();
            var lines = File.ReadAllLines(this.path);
            foreach (string line in lines)
            {
                if (!line.ContainsIgnoreCase(path))
                {
                    this.content.Add(line);
                }
            }
        }

        public void AddPathAfter(string anchor, string path)
        {
            this.content = new List<string>();
            var lines = File.ReadAllLines(this.path);
            bool added = false;
            foreach (string line in lines)
            {
                this.content.Add(line);
                string anchorProj = line.Replace("\t", "").Replace(" ", "").Replace("\\", "").Split("{").First();
                if (anchor.EqualsIgnoreCase(anchorProj))
                {
                    this.content.Add($"    {path} \\");
                    added = true;
                }
            }

            if (!added)
            {
                this.content.Add($"    {path} \\");
            }
        }

        public void Save()
        {
            var lines = new List<string>();
            foreach (var line in this.content)
            {
                string newLine = line.StartsWith(' ')
                                 ? "    " + line.Trim()
                                 : line;
                lines.Add(newLine);
            }
            File.WriteAllLines(this.path, lines);
        }
    }
}

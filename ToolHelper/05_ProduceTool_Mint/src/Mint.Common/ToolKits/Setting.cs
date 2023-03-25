namespace Mint.Common
{
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    public class Setting
    {
        private static readonly string DefaultSection = "Settings";

        public Setting(string settingFile)
        {
            this.FilePath = Path.Combine(PathUtils.ApplicationRoot(), settingFile);
        }

        public string FilePath { get; }

        public string Read(string Key, string Section = null)
        {
            var RetVal = new StringBuilder(255);
            Setting.GetPrivateProfileString(Section ?? Setting.DefaultSection, Key, "", RetVal, 255, this.FilePath);
            return RetVal.ToString();
        }

        public void Write(string Key, string Value, string Section = null)
        {
            Setting.WritePrivateProfileString(Section ?? DefaultSection, Key, Value, this.FilePath);
        }

        public void DeleteKey(string Key, string Section = null)
        {
            this.Write(Key, null, Section);
        }

        public void DeleteSection(string Section = null)
        {
            this.Write(null, null, Section);
        }

        public bool KeyExists(string Key, string Section = null)
        {
            return this.Read(Key, Section).Length > 0;
        }

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);
    }
}


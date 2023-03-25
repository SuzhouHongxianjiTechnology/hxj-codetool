namespace AlbertZhao.cn.Models
{
    public class ScanDir
    {
        private readonly string[] fileNames;
        public ScanDir()
        {
            this.fileNames = Directory.GetFiles(@"D:\ILSpy_binaries_7.1.0.6543","*",SearchOption.AllDirectories);
        }

        public int PrintFileCount()
        {
            return this.fileNames.Length;
        }
    }
}

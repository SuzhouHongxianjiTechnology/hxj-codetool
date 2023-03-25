namespace AlbertZhao.cn.Models
{
    public class FileService
    {
        public async Task SaveContent2File(string content)
        {
            await File.AppendAllTextAsync(@"F:\Test\test.txt", content);
        }
    }
}

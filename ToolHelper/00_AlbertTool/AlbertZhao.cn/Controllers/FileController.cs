using AlbertZhao.cn.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AlbertZhao.cn.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly FileService fileService;

        public FileController(FileService fileService)
        {
            this.fileService = fileService; 
        }

        [HttpPost]
        public async Task WriteContent(string content)
        {
            await fileService.SaveContent2File(content);

        }

        //对于请求慢的服务，可以通过此种方式来避免干扰到FileService的启动
        [HttpGet]
        public int GetFileCount([FromServices]ScanDir scanDir,int extraCount)
        {
            return scanDir.PrintFileCount()+ extraCount;
        }
    }
}

using AlbertZhao.cn.DbContextExtension;
using AlbertZhao.cn.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Linq;
using Albert.Commons.Interfaces;

namespace AlbertZhao.cn.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DataManagerController : ControllerBase
    {
        private readonly IMemoryCache memoryCache;
        private readonly ILogger<DataManagerController> logger;

        private readonly IDistributedCache distributedCache;
        private readonly IDistributedCacheHelper distributedCacheHelper;
        public DataManagerController(IMemoryCache memoryCache, ILogger<DataManagerController> logger, IDistributedCache distributedCache, IDistributedCacheHelper distributedCacheHelper)
        {
            this.memoryCache = memoryCache;
            this.logger = logger;
            this.distributedCache = distributedCache;
            this.distributedCacheHelper = distributedCacheHelper;
        }

        [HttpGet]
        public async Task<ActionResult<Student?>> GetStuByID(int id)
        {
            logger.LogInformation("开始查询数据....");

            //GetOrCreateAsync天然避免缓存穿透，里面会将空值作为一个有效值
            Student? stu = await memoryCache.GetOrCreateAsync("Student_" + id, async e =>
            {
                //避免缓存雪崩
                e.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(Random.Shared.Next(5, 10));

                //从数据库拿取数据
                using (var ctx = new AlbertDbContext())
                {
                    logger.LogInformation("从数据库查询数据....");
                    Student? stu = ctx.Students.Where(e => e.ID == id).FirstOrDefault();
                    logger.LogInformation($"从数据库查询的结果是:{(stu == null ? "null" : stu)}");
                    return stu;
                }
            });
            if (stu == null)
            {
                return NotFound("查询的学生不存在");
            }
            else
            {
                return stu;
            }
        }

        [HttpGet]
        public async Task<ActionResult<Student?>> GetStuRedisById(int id)
        {
            logger.LogInformation(id.ToString());
            Student? student = null;
            var s = await distributedCache.GetStringAsync("albertzhaoz" + id);
            if (s == null)
            {
                logger.LogInformation("从数据库中获取");
                if (id == 1)
                {
                    student = new Student()
                    {
                        ID = 1,
                        Name = "Albertzhao",
                        Age = 26,
                        SubName = "Albert",
                        Score = 100
                    };
                }
                else if (id == 2)
                {
                    student = new Student()
                    {
                        ID = 2,
                        Name = "Yangzhongke",
                        Age = 43,
                        SubName = "yang",
                        Score = 100
                    };
                }
                else
                {
                    student = null;
                }
                logger.LogInformation(JsonSerializer.Serialize(student));
                await distributedCache.SetStringAsync("albertzhaoz" + id, JsonSerializer.Serialize(student),
                new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(Random.Shared.Next(5, 10))
                });
            }
            else
            {
                logger.LogInformation("从Redis缓存中获取");
                student = JsonSerializer.Deserialize<Student?>(s);
            }

            if (student == null)
            {
                return NotFound("没有此学生数据");
            }
            else
            {
                return student;
            }
        }

        [HttpGet]
        public async Task<ActionResult<Student?>> GetStuRedisHelperById(int id)
        {
            logger.LogInformation(id.ToString());
            Student? student = await distributedCacheHelper.GetOrCreateAsync(
                "albertzhaoz" + id, async e =>
                   {
                       logger.LogInformation("从数据库中获取");
                       //避免缓存雪崩
                       e.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(Random.Shared.Next(5, 10));
                       Student? stuTemp = null;
                       if (id == 1)
                       {
                           stuTemp = new Student()
                           {
                               ID = 1,
                               Name = "Albertzhao",
                               Age = 26,
                               SubName = "Albert",
                               Score = 100
                           };
                       }
                       else if (id == 2)
                       {
                           stuTemp = new Student()
                           {
                               ID = 2,
                               Name = "Yangzhongke",
                               Age = 43,
                               SubName = "yang",
                               Score = 100
                           };
                       }
                       else
                       {
                           stuTemp = null;
                       }
                       return stuTemp;
                   });
            if (student == null)
            {
                return NotFound("查询的学生不存在");
            }
            else
            {
                return student;
            }
        }
    }
}

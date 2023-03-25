using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace AlbertZhao.cn.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }


        [HttpPost]
        public ActionResult<LoginResponse> LoginByUserName(LoginRequest request)
        {
            if (request.UserName == "admin" && request.Password == "123")
            {
                var items = Process.GetProcesses().Select(p => new ProcessInfo(p.Id, p.ProcessName, p.WorkingSet64));
                return new LoginResponse(true, items.ToArray());
            }
            else
            {
                return new LoginResponse(false, null);
            }
        }


        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet]
        public ActionResult<string> PrintStudent(int id)
        {
            if(id == 1)
            {
                var person = new Person("Albert", "Zhao");
                return person.PrintFullNameById(1);
            }
            else
            {
                return NotFound("人员不存在");
            }
        }

        //这样传递的好处更加符合Restful风格
        [HttpGet("{classNum}/{id}")]
        public ActionResult<string> PrintInfoByClassNumAndId(int classNum,int id)
        {
            return new Person("albert", "zhao").PrintInfo(classNum,id);
        }

        //这样传递的好处更加符合Restful风格
        [HttpGet("{schoolNum}/{id}")]
        //如果捕捉的值和函数参数名字不一致，可以用[FromRoute(Name="名字")]来指定
        public ActionResult<string> PrintInfoBySchoolNoAndId([FromRoute(Name = "schoolNum")]int schoolNo, int id)
        {
            return new Person("albert", "zhao").PrintInfo(schoolNo, id);
        }

        [HttpPost]
        public async Task<ActionResult<string>> AddPerson(Person person)
        {
            try
            {
                await System.IO.File.WriteAllTextAsync(@"F:\Test\test.txt", person.ToString());
                return "Add person successfully!";
            }
            catch (Exception)
            {
                return NotFound($"Failed!{person.ToString()}");
            }
        }

        [HttpPut("{id}")]
        public string UpdatePerson([FromRoute(Name ="id")]int id,Person p,[FromHeader(Name ="User-Agent")] string us)
        {
            return $"更新成功:{us}";
        }

        [HttpGet]
        public string LoveChen()
        {
            return "最爱小陈\n小陈真可爱\n么么哒\n";
        }
    }

    public record Person(string firstName, string lastName)
    {
        public string PrintFullNameById(int id)
        {
            return firstName + " " + lastName; 
        }

        public string PrintInfo(int classNum,int id)
        {
            return $"学生姓名为{firstName+lastName}\n" +
                $"学生班级为:{classNum}\n" +
                $"学生id为{id}\n";
        }
    }

    public record LoginRequest(string UserName,string Password);
    public record ProcessInfo(long ID,string Name,long WorkingSet);
    public record LoginResponse(bool OK, ProcessInfo[] ProcessInfos);
}
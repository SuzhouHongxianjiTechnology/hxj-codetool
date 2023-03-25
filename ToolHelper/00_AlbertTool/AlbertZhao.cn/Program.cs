using Albert.Commons.Interfaces;
using Albert.Commons.Helpers;
using Microsoft.Data.SqlClient;
using StackExchange.Redis;
using Microsoft.Extensions.DependencyInjection;
using AlbertZhao.cn.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var urls = new[] { "http://localhost:3000" }; //urls最后不要加/

#region Configuration
//Zack.AnyDBConfigProvider里面的GetConnectionString读取的字符串是配置文件中的
//而不是数据库中的,连接上数据库后，AddDbConfiguration才是从数据库中读取Json
//这里Host和WebHost一个是主机一个是通用主机
//如果想要启用builder.Configuration.GetConnectionString("con");
//则需要在appsettings.json中配置“ConnectionStrings:con":"xxx"
builder.WebHost.ConfigureAppConfiguration((hostCtx, configBuilder) => {
    var env = hostCtx.HostingEnvironment;
    var connStr = builder.Configuration.GetValue<string>("SqlServer:ConnectStr");
    configBuilder.AddDbConfiguration(() =>
    new SqlConnection(connStr),
    reloadOnChange: true,
    reloadInterval: TimeSpan.FromSeconds(2),
    tableName: "ProduceToolConfig");

    //开发模式下，保存项目程序集到用户机密
    if (env.IsDevelopment() && !string.IsNullOrEmpty(env.ApplicationName))
    {
        var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
        if (appAssembly != null)
        {
            configBuilder.AddUserSecrets(appAssembly, optional: true);
        }
    }
});
//AddDbConfiguration之后即可从数据库中拿取Json对了
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var connStr = builder.Configuration.GetValue<string>("RedisServer:Configuration");
    return ConnectionMultiplexer.Connect(connStr);
});
//配置选项：Smtp 注意这里的根是appsettings.json
builder.Services.AddOptions().Configure<SmtpSettings>(smtp => builder.Configuration.GetSection("SmtpSettings").Bind(smtp));
#endregion

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//依赖注入
builder.Services.AddFileService();
builder.Services.AddScanDir();

//内存缓存
builder.Services.AddMemoryCache();

//启用分布式缓存Redis
builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = "localhost";
    options.InstanceName = "albertzhaoz_";
});


//启用分布式缓存Albert帮助类，更加方便的使用GetOrCreateAsync方法和类型限制
builder.Services.AddScoped<IDistributedCacheHelper, DistributedCacheHelper>();

// CORS策略
builder.Services.AddCors(options =>
   options.AddDefaultPolicy(builder =>
       builder.WithOrigins(urls).AllowAnyMethod().AllowAnyHeader().AllowCredentials()));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//运行环境读取方法
if (app.Environment.EnvironmentName == "AlbertTest")
{
    Console.WriteLine(app.Environment.EnvironmentName);
}

//启用中间件 CORS策略：跨域策略
app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

//app.UseResponseCaching(); //启用中间件 CORS策略：跨域策略

app.MapControllers();

app.Run();

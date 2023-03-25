using Albert.Extensions;
using Albert.Interface;
using CliFx;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DI_GitExtensions
    {
        public static void AddGitExtensions(this IServiceCollection service)
        {
            //这里将GitExtension注册进去,用于CliFx取服务对象
            service.AddScoped<GitExtension>();
        }
    }
}

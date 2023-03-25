using AlbertZhao.cn.Models;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DI_FileService
    {
        public static void AddFileService(this IServiceCollection services)
        {
            services.AddScoped<FileService>();
        }
    }
}

using AlbertZhao.cn.Models;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DI_ScanDir
    {
        public static void AddScanDir(this IServiceCollection services)
        {
            services.AddScoped<ScanDir>();
        }
    }
}

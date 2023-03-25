using Albert.Extensions;
using Albert.Interface;
using CliFx;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DI_BagetExtensions
    {
        public static void AddBagetExtensions(this IServiceCollection service)
        {
            service.AddScoped<BagetExtension>();
        }
    }
}

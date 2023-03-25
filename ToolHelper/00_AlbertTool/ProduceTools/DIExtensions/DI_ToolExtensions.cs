using Albert.Extensions;
using Albert.Interface;
using CliFx;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DI_ToolExtensions
    {
        public static void AddToolExtensions(this IServiceCollection service)
        {
            service.AddScoped<ToolExtension>();
        }
    }
}

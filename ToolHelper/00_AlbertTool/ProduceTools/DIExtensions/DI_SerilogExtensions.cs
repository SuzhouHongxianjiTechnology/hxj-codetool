using Albert.Extensions;
using Albert.Interface;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DI_SerilogExtensions
    {
        public static void AddSerilogExtensions(this IServiceCollection service)
        {
            service.AddScoped<ISeriLog, SerilogExtension>();
        }
    }
}

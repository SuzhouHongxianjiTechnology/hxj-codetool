using Albert.Extensions;
using Albert.Interface;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DI_GetHelperExtensions
    {
        public static void AddGetHelperExtensions(this IServiceCollection service)
        {
            service.AddScoped<IHelper, HelperInfoExtension>();
        }
    }
}

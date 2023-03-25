using Albert.Extensions;
using Albert.Interface;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DI_SimpleCrawlerExtensions
    {
        public static void AddSimpleCrawlerExtensions(this IServiceCollection service)
        {
            service.AddScoped<ICrawler, SimpleCrawlerExtension>();
        }
    }
}

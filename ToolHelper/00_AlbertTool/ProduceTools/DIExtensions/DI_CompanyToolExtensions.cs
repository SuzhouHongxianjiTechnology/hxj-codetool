using Albert.Extensions;
using Albert.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DI_CompanyToolExtensions
    {
        public static void AddCompanyToolExtensions(this IServiceCollection service)
        {
            service.AddScoped<ICompanyTool, CompanyToolExtensions>();
        }
    }
}

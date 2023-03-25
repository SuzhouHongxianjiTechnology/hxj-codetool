using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Albert.Interface
{
    interface IProduce
    {
        void RunProduceExtensions(IServiceProvider sp, string[] args);
    }
}

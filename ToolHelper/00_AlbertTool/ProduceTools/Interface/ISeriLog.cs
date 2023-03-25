using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Albert.Interface
{
    public interface ISeriLog
    {
        public string ExceptionlessClientDefaultStartUpKey { get; set; }
        public string SerilogFilePath { get; set; }
        bool OpenExceptionlessClient();
    }
}

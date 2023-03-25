using Albert.Interface;
using Albert.Model;
using Microsoft.Extensions.Options;

namespace Albert.Extensions
{
    public class SerilogExtension: ISeriLog
    {
        private readonly IOptionsSnapshot<ProduceToolEntity> options;
        public string ExceptionlessClientDefaultStartUpKey { get; set; }
        public string SerilogFilePath { get; set; }

        public SerilogExtension(IOptionsSnapshot<ProduceToolEntity> options)
        {
            this.options = options;
            this.ExceptionlessClientDefaultStartUpKey = options.Value.SerilogConfig.ExceptionlessClientDefaultStartUpKey;
            this.SerilogFilePath = options.Value.SerilogConfig.SerilogFilePath;
        }

        public bool OpenExceptionlessClient()
        {
            if (string.IsNullOrEmpty(this.options.Value.SerilogConfig.ExceptionlessClientDefaultStartUpKey))
            {
                return false;
            }
            return true;
        }
    }
}

using Albert.Interface;
using Albert.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Albert.Extensions
{
    public class HelperInfoExtension: IHelper
    {
        private readonly IOptionsSnapshot<ProduceToolEntity> options;
        public HelperInfoExtension(IOptionsSnapshot<ProduceToolEntity> options)
        {
            this.options= options;
        }

        public void RunHelperInfoExtension(IServiceProvider sp, string[] args)
        {
            if((args.Length > 0) && args[0].Contains("help"))
            {
                Console.WriteLine(options.Value.HelperInfo.CmdInformation);
                Console.WriteLine(options.Value.HelperInfo.RemoteRepoAddress);
            }           
        }     
    }
}

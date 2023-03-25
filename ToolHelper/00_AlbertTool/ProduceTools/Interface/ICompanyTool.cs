using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Albert.Interface
{
    internal interface ICompanyTool
    {
        void EnterProducePath(string path); //进入Produce netcore的目录

        void ExcuteGdepsF();//执行gdeps -f下载依赖项
        void ExcuteMsbuildRestore();//执行msbuild -t:restore进行项目恢复
        void ExcuteBcc();//执行bcc，进行项目编译
        void ExcuteBccr();//执行bccr,进行release版本的编译
        void ExcuteGetDepsFlavorRetail();//执行getdeps /flavors:retail，进行retail所需项下载
        Task RunCompanyToolExtensions(IServiceProvider sp, string[] args);//启动工具
    }
}

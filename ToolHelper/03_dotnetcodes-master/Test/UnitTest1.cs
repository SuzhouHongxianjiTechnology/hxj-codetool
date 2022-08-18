/*
 源码己托管:https://gitee.com/kuiyu/dotnetcodes
 */

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotNet.Utilities;
 
namespace Test
{
    [TestClass]
    public class UnitTest1
    {



        [TestMethod]
        public void TestMethod1()
        {
            double money = 156.23;
            int count = 39;
            var m= RedPacket.GetMoneys(money,count);
            var n = m;
            double t = 0;
            for (int i = 0; i < n.Count; i++)
            {
                t += n[i];
            }
            var r = t;
        }
    }
}

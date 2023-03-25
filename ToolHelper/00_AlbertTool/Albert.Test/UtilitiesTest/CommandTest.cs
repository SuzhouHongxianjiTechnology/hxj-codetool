using Albert.Commons.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Albert.Test
{
    [TestClass]
    public class CommandTest
    {
        //初始化
        //[TestInitialize]
        //public void Setup()
        //{
        //    DataReceiveList = Command.DataReceiveLis;
        //}
        //直接使用MSTest.Unit

        [TestMethod]
        public void TestExecuteCmd()
        {
            CommandHelper.ExecuteCmd("dotnet --version");
            var str = "6.0.100";
            Assert.AreEqual(str, CommandHelper.DataReceiveList[4]);
        }
    }
}

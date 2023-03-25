namespace Mint.Substrate.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mint.Substrate.Utilities;

    [TestClass]
    public class SubstrateUtilsTest
    {
        [TestMethod]
        public void TestGetCompDirSrc()
        {
            string baseDir = @"D:\repo\merge\src\sources\dev\cafe\src\RoutingService\Client";
            string expected = @"D:\repo\merge\src\sources\dev\cafe\src";
            string result = SubstrateUtils.GetCompDirSrc(baseDir);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestResolveVariables()
        {
            string baseDir = @"D:\repo\merge\src\sources\dev\cafe\src\RoutingService\Client";
            string result, expected;

            result = SubstrateUtils.ResolveVariables(baseDir, @"$(??????)");
            Assert.AreEqual(@"$(??????)", result);

            result = SubstrateUtils.ResolveVariables(baseDir, @"$(COMPDIRSRC)");
            expected = @"D:\repo\merge\src\sources\dev\cafe\src";
            Assert.AreEqual(expected, result);

            result = SubstrateUtils.ResolveVariables(baseDir, @"$(MSBuildProjectDirectory)");
            expected = @"D:\repo\merge\src\sources\dev\cafe\src\RoutingService\Client";
            Assert.AreEqual(expected, result);

            result = SubstrateUtils.ResolveVariables(baseDir, @"$(SourcesRootDir)");
            expected = "";
            Assert.AreEqual(expected, result);

            result = SubstrateUtils.ResolveVariables(baseDir, @"$(ROOT)");
            expected = @"D:\repo\merge\src";
            Assert.AreEqual(expected, result);

            result = SubstrateUtils.ResolveVariables(baseDir, @"$(INETROOT)");
            expected = @"D:\repo\merge\src";
            Assert.AreEqual(expected, result);
        }
    }
}

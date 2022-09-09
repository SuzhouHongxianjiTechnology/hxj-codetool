namespace Mint.Common.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mint.Common.Utilities;

    [TestClass]
    public class PathUtilsTest
    {

        [DataTestMethod]
        [DataRow(@"C:\root\sub\inner\some.file", @"C:\root\sub\", @"inner\some.file")]
        [DataRow(@"C:\root\sub\inner\some.file", @"C:\root\sub\", @"\inner\some.file")]
        [DataRow(@"C:\root\sub\inner\some.file", @"C:\root\sub\", @"..\sub\inner\some.file")]
        [DataRow(@"C:\root\sub\inner\some.file", @"C:\root\sub\", @"..\..\root\sub\inner\some.file")]
        public void Test_GetAbsolutePath(string fullPath, string parent, string relativePath)
        {
            Assert.AreEqual(fullPath, PathUtils.GetAbsolutePath(parent, relativePath));
        }

        [DataTestMethod]
        [DataRow(@"C:\root\sub\inner\some.file", @"C:\root\sub\", @"inner\some.file")]
        [DataRow(@"C:\root\sub\inner\some.file", @"C:\root\other\", @"..\sub\inner\some.file")]
        public void Test_GetRelativePath(string fullPath, string parent, string relativePath)
        {
            Assert.AreEqual(relativePath, PathUtils.GetRelativePath(parent, fullPath));
        }
    }
}

namespace Mint.Common.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mint.Common;

    [TestClass]
    public class PathUtilsTest
    {

        [TestMethod]
        public void TestToAbsolutePath()
        {
            string fullpath = @"C:\root\sub\inner\some.file";
            string parent = @"C:\root\sub\";

            string path1 = @"inner\some.file";
            Assert.AreEqual(fullpath, PathUtils.GetAbsolutePath(parent, path1));

            string path2 = @"\inner\some.file";
            Assert.AreEqual(fullpath, PathUtils.GetAbsolutePath(parent, path2));

            string path3 = @"..\sub\inner\some.file";
            Assert.AreEqual(fullpath, PathUtils.GetAbsolutePath(parent, path3));

            string path4 = @"..\..\root\sub\inner\some.file";
            Assert.AreEqual(fullpath, PathUtils.GetAbsolutePath(parent, path4));
        }

        [TestMethod]
        public void TestToRelativePath()
        {
            string fullpath = @"C:\root\sub\inner\some.file";

            string parent1 = @"C:\root\sub\";
            Assert.AreEqual(@"inner\some.file", PathUtils.GetRelativePath(parent1, fullpath));

            string parent2 = @"C:\root\other\";
            Assert.AreEqual(@"..\sub\inner\some.file", PathUtils.GetRelativePath(parent2, fullpath));
        }
    }
}

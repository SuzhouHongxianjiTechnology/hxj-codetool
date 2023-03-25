namespace Mint.Common.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mint.Common;

    [TestClass]
    public class StringUtilsTest
    {
        [TestMethod]
        public void TestStringEqualsIgnoreCase()
        {
            string s1 = "{ABCD}";
            string s2 = "{abcd}";
            Assert.IsTrue(StringUtils.EqualsIgnoreCase(s1, s2));
        }

        [TestMethod]
        public void TestStringStartsWithIgnoreCase()
        {
            string s = "abcdefg";
            Assert.IsTrue(StringUtils.StartsWithIgnoreCase(s, "ABCD"));
        }

        [TestMethod]
        public void TestStringEndsWithIgnoreCase()
        {
            string s = "abcdefg";
            Assert.IsTrue(StringUtils.EndsWithIgnoreCase(s, "efg"));
        }

        [TestMethod]
        public void TestStringContainsIgnoreCase()
        {
            string s = "abcdefg";
            Assert.IsTrue(StringUtils.ContainsIgnoreCase(s, "CDE"));

            s = @"$(COMPDIRSRC)";
            Assert.IsTrue(StringUtils.ContainsIgnoreCase(s, "$(COMPDIRSRC)"));
        }

        [TestMethod]
        public void TestStringReplaceIgnoreCase()
        {
            string s = "abcdefg";
            string v = StringUtils.ReplaceIgnoreCase(s, "CDE", "xxx");
            Assert.AreEqual("abxxxfg", v);

            s = @"$(COMPDIRSRC)";
            v = StringUtils.ReplaceIgnoreCase(s, @"$(COMPDIRSRC)", "xxx");
            Assert.AreEqual("xxx", v);
        }
    }
}

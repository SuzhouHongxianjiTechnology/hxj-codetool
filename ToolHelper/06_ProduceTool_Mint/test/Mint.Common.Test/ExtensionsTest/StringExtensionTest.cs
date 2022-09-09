namespace Mint.Common.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mint.Common.Extensions;

    [TestClass]
    public class StringExtensionTest
    {
        [TestMethod]
        public void Test_StringEqualsIgnoreCase()
        {
            string s1 = "{ABCD}";
            string s2 = "{abcd}";
            Assert.IsTrue(s1.EqualsIgnoreCase(s2));

            s1 = "init.key.3";
            s2 = "INIT.KEY.3";
            Assert.IsTrue(s1.EqualsIgnoreCase(s2));

            s1 = null;
            s2 = "123";
            Assert.IsFalse(s1.EqualsIgnoreCase(s2));

            s1 = "123";
            s2 = null;
            Assert.IsFalse(s1.EqualsIgnoreCase(s2));

            s1 = @"$(RootNamespace)";
            s2 = "$(rootnamespace)";
            Assert.IsTrue(s1.EqualsIgnoreCase(s2));
        }

        [TestMethod]
        public void Test_StringStartsWithIgnoreCase()
        {
            string s = "abcdefg";
            Assert.IsTrue(s.StartsWithIgnoreCase("ABCD"));
        }

        [TestMethod]
        public void Test_StringEndsWithIgnoreCase()
        {
            string s = "abcdefg";
            Assert.IsTrue(s.EndsWithIgnoreCase("efg"));
        }

        [TestMethod]
        public void Test_StringContainsIgnoreCase()
        {
            string s = "Microsoft.Exchange.Pop3.EventLog";
            Assert.IsTrue(s.ContainsIgnoreCase("xchange.Pop3.EventL"));

            s = @"$(COMPDIRSRC)";
            Assert.IsTrue(s.ContainsIgnoreCase("$(COMPDIRSRC)"));

            s = @"    Imap4.NetStd \";
            Assert.IsTrue(s.ContainsIgnoreCase("imap4.netstd"));

            s = @"    Imap4.NetCore \";
            Assert.IsTrue(s.ContainsIgnoreCase("imap4.netcore"));
        }

        [TestMethod]
        public void Test_StringReplaceIgnoreCase()
        {
            string s = "abcdefg";
            string v = s.ReplaceIgnoreCase("CDE", "xxx");
            Assert.AreEqual("abxxxfg", v);

            s = @"$(COMPDIRSRC)";
            v = s.ReplaceIgnoreCase(@"$(COMPDIRSRC)", "xxx");
            Assert.AreEqual("xxx", v);

            s = @"''""""||^^$$//\\";
            v = s.ReplaceIgnoreCase(@"''""""||^^$$//\\", @"\\//$$^^||""""''");
            Assert.AreEqual(@"\\//$$^^||""""''", v);

            s = "string";
            v = s.ReplaceIgnoreCase(null, "a");
            Assert.AreEqual(s, v);

            s = "string";
            v = s.ReplaceIgnoreCase("a", null);
            Assert.AreEqual(s, v);
        }

        [TestMethod]
        public void Test_StringSplitIgnoreCase()
        {
            string s = "abcd$^efg$^kkk";
            string[] parts = s.SplitIgnoreCase("$^");

            Assert.AreEqual("abcd", parts[0]);
            Assert.AreEqual("efg", parts[1]);
            Assert.AreEqual("kkk", parts[2]);
        }
    }
}

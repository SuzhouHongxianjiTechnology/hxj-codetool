namespace Mint.Common.Test
{
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mint.Common.Collections;

    [TestClass]
    public class IgnoreCaseDictionaryTest
    {
        IgnoreCaseDictionary<string> dict;

        [TestInitialize]
        public void Setup()
        {
            dict = new IgnoreCaseDictionary<string>
            {
                {"init.key.1", "value1"},
                {"init.key.2", "value3"},
                {"init.key.3", "value2"}
            };
        }

        [TestMethod]
        public void Test_insert_new_key_value()
        {
            Assert.AreEqual(3, dict.Count);

            dict["new key"] = "new value";

            Assert.AreEqual(4, dict.Count);
            Assert.AreEqual("new value", dict["new key"]);
        }

        [TestMethod]
        public void Test_insert_exists_key_value()
        {
            Assert.AreEqual(3, dict.Count);
            Assert.AreEqual("value1", dict["init.key.1"]);
            Assert.AreEqual("value1", dict["INIT.KEY.1"]);

            dict["INIT.KEY.1"] = "new value";
            Assert.AreEqual(3, dict.Count);

            string actualKey = dict.Keys.FirstOrDefault();
            Assert.AreEqual(actualKey, "init.key.1");
        }

        [TestMethod]
        public void Test_try_get_value()
        {
            bool found = dict.TryGetValue("INIT.KEY.1", out string value);
            Assert.IsTrue(found);
            Assert.AreEqual("value1", value);
        }

        [TestMethod]
        public void Test_try_get_key()
        {
            bool found = false;
            string actualKey;

            found = dict.TryGetKey("init.key.1", out actualKey);
            Assert.IsTrue(found);
            Assert.AreEqual("init.key.1", actualKey);

            found = dict.TryGetKey("INIT.KEY.3", out actualKey);
            Assert.IsTrue(found);
            Assert.AreEqual("init.key.3", actualKey);
        }
    }
}

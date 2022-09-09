namespace Mint.Common.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mint.Common.Utilities;

    [TestClass]
    public class UserInputParserTest
    {
        [TestMethod]
        public void Test_user_input_with_args()
        {
            string[] args = { "some_command", "some_arg1", "some_arg2" };
            UserInputParser.Parse(args, out string command, out string argument1, out string argument2);

            Assert.AreEqual("some_command", command);
            Assert.AreEqual("some_arg1", argument1);
            Assert.AreEqual("some_arg2", argument2);
        }

        [TestMethod]
        public void Test_user_input_with_no_args()
        {
            string[] args = null;
            UserInputParser.Parse(args, out string command, out string argument1, out string argument2);

            Assert.AreEqual(string.Empty, command);
            Assert.AreEqual(string.Empty, argument1);
            Assert.AreEqual(string.Empty, argument2);
        }
    }
}

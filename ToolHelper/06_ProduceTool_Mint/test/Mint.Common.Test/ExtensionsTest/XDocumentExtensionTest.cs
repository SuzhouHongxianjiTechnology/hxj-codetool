namespace Mint.Common.Test
{
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mint.Common.Extensions;

    [TestClass]
    public class XDocumentExtensionTest
    {
        XDocument document;

        [TestInitialize]
        public void Setup()
        {
            document = XDocument.Parse(@"
                <Project>
                    <PropertyGroup>
                        <Property1>Property1</Property1>
                        <Property2>Property2</Property2>
                        <Property3>Property3</Property3>
                        <Property4>Property4</Property4>
                    </PropertyGroup>
                    <ItemGroup Id=""ItemGroup1"">
                        <Item Include=""Group1_Item1"" />
                        <Item Include=""Group1_Item2"" />
                        <Item Include=""Group1_Item3"" />
                    </ItemGroup>
                    <ItemGroup Id=""ItemGroup2"">
                        <Item Include=""Group2_Item4"" />
                        <Item Include=""Group2_Item5"" />
                        <Item Include=""Group2_Item6"" />
                    </ItemGroup>
                </Project>
            ");
        }

        [TestMethod]
        public void Test_GetFirst_should_return_the_first_element_that_matchs_tag()
        {
            var item = document.GetFirst("item");
            string expected = @"<Item Include=""Group1_Item1"" />";
            Assert.AreEqual(expected, item.ToString());
        }

        [TestMethod]
        public void Test_GetFirst_should_return_null_if_no_matched_tag()
        {
            var item = document.GetFirst("not_exists");
            Assert.IsNull(item);
        }

        [TestMethod]
        public void Test_GetAll_should_return_elements_by_tags_ignore_case()
        {
            var groups = document.GetAll("iTemGroup");
            Assert.AreEqual(2, groups.Count());

            var items = document.GetAll("item");
            Assert.AreEqual(6, items.Count());
        }

        [TestMethod]
        public void Test_GetAll_should_return_empty_enumerable_if_tags_not_exists()
        {
            var none = document.GetAll("not_exists");
            Assert.IsNotNull(none);
            Assert.AreEqual(0, none.Count());
        }
    }
}

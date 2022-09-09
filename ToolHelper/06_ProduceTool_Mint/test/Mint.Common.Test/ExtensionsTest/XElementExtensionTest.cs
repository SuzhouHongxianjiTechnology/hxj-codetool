namespace Mint.Common.Test
{
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mint.Common.Extensions;

    [TestClass]
    public class XElementExtensionTest
    {
        XElement element;

        [TestInitialize]
        public void Setup()
        {
            element = XElement.Parse(@"
            <ItemGroup>
                <Item Include=""Item1"" />
                <Item Include=""Item2"" />
                <OtherTag />
                <Item Include=""Item3"" />
                <Item Include=""Item4"">
                    <SubItem />
                </Item>
            </ItemGroup>
            ");
        }

        [TestMethod]
        public void Test_Clone_should_return_a_string_copy_of_that_element()
        {
            var clone = element.Clone();
            Assert.AreEqual(element.ToString(), clone.ToString());
        }

        [TestMethod]
        public void Test_Is_should_return_true_if_element_matchs_tag_ignore_case()
        {
            Assert.IsTrue(element.Is("itemgroup"));
            Assert.IsTrue(element.Is("ITEMGROUP"));
        }

        [TestMethod]
        public void Test_Is_should_return_false_if_element_tag_is_not_matched()
        {
            Assert.IsFalse(element.Is("asdfadf"));
        }

        [TestMethod]
        public void Test_TryRemove_should_return_true_if_element_has_no_parent()
        {
            Assert.IsNull(element.Parent);
            Assert.IsTrue(element.TryRemove());
        }

        [TestMethod]
        public void Test_TryRemove_should_remove_element_from_its_parent()
        {
            var item = element.GetFirst("Item");
            Assert.IsNotNull(item.Parent);
            Assert.IsTrue(item.TryRemove());
            Assert.IsNull(item.Parent);
        }

        [TestMethod]
        public void Test_RemoveIfEmpty_should_remove_element_if_it_has_no_children()
        {
            Assert.AreEqual(5, element.Elements().Count());

            var item1 = element.Elements().First();
            Assert.IsFalse(item1.Elements().Any());

            item1.RemoveIfEmpty();
            Assert.AreEqual(4, element.Elements().Count());
        }

        [TestMethod]
        public void Test_RemoveIfEmpty_should_not_remove_element_if_it_has_children()
        {
            Assert.AreEqual(5, element.Elements().Count());

            var item4 = element.Elements().Last();
            Assert.IsTrue(item4.Elements().Any());

            item4.RemoveIfEmpty();
            Assert.AreEqual(5, element.Elements().Count());
        }

        [TestMethod]
        public void Test_GetFirst_should_return_the_first_child_of_that_tag_ignore_case()
        {
            var item1 = element.GetFirst("item");
            string expected = @"<Item Include=""Item1"" />";
            Assert.AreEqual(expected, item1.ToString());
        }

        [TestMethod]
        public void Test_GetFirst_should_return_null_if_no_tag_match()
        {
            var item = element.GetFirst("asdf");
            Assert.IsNull(item);
        }

        [TestMethod]
        public void Test_GetAll_should_return_all_elements_match_tag()
        {
            var items = element.GetAll("item");
            Assert.AreEqual(4, items.Count());
        }

        [TestMethod]
        public void Test_GetAll_should_return_empty_enumerable_if_no_child_matchs_tag()
        {
            var items = element.GetAll("asdfasdf");
            Assert.IsNotNull(items);
            Assert.AreEqual(0, items.Count());
        }

        [TestMethod]
        public void Test_LastSibling_should_return_the_last_sibling_of_that_element()
        {
            var item = element.GetFirst("OtherTag");
            string expected = @"<Item Include=""Item2"" />";
            Assert.AreEqual(expected, item.LastSibling().ToString());
        }

        [TestMethod]
        public void Test_LastSibling_should_return_null_if_element_is_the_first_one()
        {
            var first = element.GetAll("item").First();
            Assert.IsNull(first.LastSibling());
        }

        [TestMethod]
        public void Test_NextSibling_should_return_the_next_sibling_of_that_element()
        {
            var item = element.GetFirst("OtherTag");
            string expected = @"<Item Include=""Item3"" />";
            Assert.AreEqual(expected, item.NextSibling().ToString());
        }

        [TestMethod]
        public void Test_NextSibling_should_return_null_if_element_is_the_last_one()
        {
            var last = element.GetAll("item").Last();
            Assert.IsNull(last.NextSibling());
        }

        [TestMethod]
        public void Test_HasAttribute_should_return_true_if_element_has_specific_attribute()
        {
            var item = element.Elements("Item").First();
            Assert.IsTrue(item.HasAttribute("include"));
        }

        [TestMethod]
        public void Test_HasAttribute_should_return_false_if_element_does_not_have_specific_attribute()
        {
            var item = element.Elements("Item").First();
            Assert.IsFalse(item.HasAttribute("somethingelse"));
        }

        [TestMethod]
        public void Test_HasAttribute_should_return_true_if_element_has_exactly_the_attribute()
        {
            var item = element.Elements("Item").Last();
            Assert.IsTrue(item.HasAttribute("include", "item4"));
        }

        [TestMethod]
        public void Test_HasAttribute_should_return_false_if_the_attribute_is_not_same()
        {
            var item = element.Elements("Item").Last();
            Assert.IsFalse(item.HasAttribute("include", "other"));
        }

        [TestMethod]
        public void Test_HasAttribute_should_use_trimed_attribute_value_for_comparsion()
        {
            var item = XElement.Parse(@"<Compile Condition=""   $(DefineConstants.Contains('NETFRAMEWORK'))"" />");
            Assert.IsTrue(item.HasAttribute("Condition", "$(DefineConstants.Contains('NETFRAMEWORK'))   "));
        }
    }
}

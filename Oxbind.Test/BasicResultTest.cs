namespace Maroontress.Oxbind.Test;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class BasicResultTest
{
    [TestMethod]
    public void RootTest()
    {
        const string xml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <root>
              <first value="42"/>
              <second>10</second>
              <second>20</second>
            </root>
            """;
        var factory = new OxbinderFactory();
        var binder = factory.Of<AttributeRoot>();
        var reader = new StringReader(xml);
        var root = binder.NewInstance(reader);
        root.Test();
    }

    [ForElement("root")]
    public record class AttributeRoot(
        [Required] BindResult<First> FirstChild,
        [Multiple] IEnumerable<BindResult<Second>> SecondChildren)
    {
        public void Test()
        {
            _ = FirstChild ?? throw new NullReferenceException();
            _ = SecondChildren ?? throw new NullReferenceException();
            var first = FirstChild.Value;
            Assert.AreEqual(3, FirstChild.Line);
            Assert.AreEqual(4, FirstChild.Column);
            var firstValue = first.AttributeValue;
            Assert.AreEqual("42", firstValue.Value);
            Assert.AreEqual(3, firstValue.Line);
            Assert.AreEqual(10, firstValue.Column);

            var list = SecondChildren.ToList();
            Assert.AreEqual(2, list.Count);
            {
                var second10 = list[0];
                var second10Value = second10.Value;
                Assert.AreEqual(4, second10.Line);
                Assert.AreEqual(4, second10.Column);
                var testValue10 = second10Value.InnerText;
                Assert.AreEqual("10", testValue10.Value);
                Assert.AreEqual(4, testValue10.Line);
                Assert.AreEqual(11, testValue10.Column);
            }
            {
                var second20 = list[1];
                var second20Value = second20.Value;
                Assert.AreEqual(5, second20.Line);
                Assert.AreEqual(4, second20.Column);
                var testValue20 = second20Value.InnerText;
                Assert.AreEqual("20", testValue20.Value);
                Assert.AreEqual(5, testValue20.Line);
                Assert.AreEqual(11, testValue20.Column);
            }
        }
    }

    [ForElement("first")]
    public record class First(
        [ForAttribute("value")] BindResult<string> AttributeValue);

    [ForElement("second")]
    public record class Second(
        [ForText] BindResult<string> InnerText);
}

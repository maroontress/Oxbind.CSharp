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

        var firstChild = root.FirstChild
            ?? throw new NullReferenceException();
        var secondChildren = root.SecondChildren
            ?? throw new NullReferenceException();
        var first = firstChild.Value;
        Assert.AreEqual(3, firstChild.Line);
        Assert.AreEqual(4, firstChild.Column);
        var firstValue = first.AttributeValue;
        Assert.AreEqual("42", firstValue.Value);
        Assert.AreEqual(3, firstValue.Line);
        Assert.AreEqual(10, firstValue.Column);

        var list = secondChildren.ToList();
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

    [TestMethod]
    public void RequiredForChildElement()
    {
        const string xml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <root>
              <foo>Hello</foo>
              <bar>World</bar>
            </root>
            """;
        var factory = new OxbinderFactory();
        var binder = factory.Of<FooBarRoot>();
        var reader = new StringReader(xml);
        var root = binder.NewInstance(reader);
        {
            var foo = root.Foo;
            Assert.AreEqual("Hello", foo.Value);
            Assert.AreEqual(3, foo.Line);
            Assert.AreEqual(8, foo.Column);
        }
        {
            var bar = root.Bar;
            Assert.AreEqual("World", bar.Value);
            Assert.AreEqual(4, bar.Line);
            Assert.AreEqual(8, bar.Column);
        }
    }

    [TestMethod]
    public void OptionalMultipleForChildElement()
    {
        const string xml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <root>
              <foo>Hello, World</foo>
              <bar>firstBar</bar>
              <bar>secondBar</bar>
            </root>
            """;
        var factory = new OxbinderFactory();
        var binder = factory.Of<OptionalFooMultipleBarRoot>();
        var reader = new StringReader(xml);
        var root = binder.NewInstance(reader);

        Assert.AreEqual("Hello, World", root.Foo?.Value);
        var list = root.Bar.ToList();
        Assert.AreEqual(2, list.Count);
        {
            var firstItem = list[0];
            Assert.AreEqual("firstBar", firstItem.Value);
            Assert.AreEqual(4, firstItem.Line);
            Assert.AreEqual(8, firstItem.Column);
        }
        {
            var secondItem = list[1];
            Assert.AreEqual("secondBar", secondItem.Value);
            Assert.AreEqual(5, secondItem.Line);
            Assert.AreEqual(8, secondItem.Column);
        }
        Assert.IsNull(root.Baz);
    }

    [ForElement("root")]
    public record class AttributeRoot(
        [Required] BindResult<First> FirstChild,
        [Multiple] IEnumerable<BindResult<Second>> SecondChildren);

    [ForElement("first")]
    public record class First(
        [ForAttribute("value")] BindResult<string> AttributeValue);

    [ForElement("second")]
    public record class Second(
        [ForText] BindResult<string> InnerText);

    [ForElement("root")]
    public record class FooBarRoot(
        [Required][ForChildElement("foo")] BindResult<string> Foo,
        [Required][ForChildElement("bar")] BindResult<string> Bar);

    [ForElement("root")]
    public record class OptionalFooMultipleBarRoot(
        [Optional][ForChildElement("foo")] BindResult<string>? Foo,
        [Multiple][ForChildElement("bar")] IEnumerable<BindResult<string>> Bar,
        [Optional][ForChildElement("baz")] BindResult<string>? Baz);
}

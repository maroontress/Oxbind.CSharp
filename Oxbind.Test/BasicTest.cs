namespace Maroontress.Oxbind.Test;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class BasicTest
{
    [TestMethod]
    public void RequiredOptionalOneMultiple()
    {
        const string xml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <root>
              <first value="42"/>
              <second>text</second>
              <third>10</third>
              <third>20</third>
            </root>
            """;
        var factory = new OxbinderFactory();
        var binder = factory.Of<RequiredOptionalMultipleRoot>();
        var reader = new StringReader(xml);
        var root = binder.NewInstance(reader);

        _ = root.FirstChild ?? throw new NullReferenceException();
        Assert.AreEqual("42", root.FirstChild.AttributeValue);
        _ = root.SecondChild ?? throw new NullReferenceException();
        Assert.AreEqual("text", root.SecondChild.InnerText);
        _ = root.ThirdChildren ?? throw new NullReferenceException();
        Assert.AreEqual(2, root.ThirdChildren.Count());
        var list = root.ThirdChildren.Select(e => e.InnerText)
            .ToList();
        var expect = new[] { "10", "20" };
        Assert.AreEqual(expect[0], list[0]);
        Assert.AreEqual(expect[1], list[1]);
    }

    [TestMethod]
    public void RequiredOptionalZeroMultiple()
    {
        const string xml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <root>
              <first value="42"/>
              <third>10</third>
              <third>20</third>
            </root>
            """;
        var factory = new OxbinderFactory();
        var binder = factory.Of<RequiredOptionalMultipleRoot>();
        var reader = new StringReader(xml);
        var root = binder.NewInstance(reader);

        _ = root.FirstChild ?? throw new NullReferenceException();
        _ = root.ThirdChildren ?? throw new NullReferenceException();
        Assert.AreEqual("42", root.FirstChild.AttributeValue);
        Assert.IsNull(root.SecondChild);
        Assert.AreEqual(2, root.ThirdChildren.Count());
        var list = root.ThirdChildren.Select(e => e.InnerText)
            .ToList();
        var expect = new[] { "10", "20" };
        Assert.AreEqual(expect[0], list[0]);
        Assert.AreEqual(expect[1], list[1]);
    }

    [TestMethod]
    public void RequiredMultipleOptionalOne()
    {
        const string xml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <root>
              <first value="42"/>
              <second>10</second>
              <second>20</second>
              <third>text</third>
            </root>
            """;
        var factory = new OxbinderFactory();
        var binder = factory.Of<RequiredMultipleOptionalRoot>();
        var reader = new StringReader(xml);
        var root = binder.NewInstance(reader);

        _ = root.FirstChild ?? throw new NullReferenceException();
        _ = root.SecondChildren ?? throw new NullReferenceException();
        Assert.AreEqual("42", root.FirstChild.AttributeValue);
        Assert.AreEqual(2, root.SecondChildren.Count());
        var list = root.SecondChildren.Select(e => e.InnerText)
            .ToList();
        var expect = new[] { "10", "20" };
        Assert.AreEqual(expect[0], list[0]);
        Assert.AreEqual(expect[1], list[1]);
        _ = root.ThirdChild ?? throw new NullReferenceException();
        Assert.AreEqual("text", root.ThirdChild.InnerText);
    }

    [TestMethod]
    public void RequiredMultipleOptionalZero()
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
        var binder = factory.Of<RequiredMultipleOptionalRoot>();
        var reader = new StringReader(xml);
        var root = binder.NewInstance(reader);

        _ = root.FirstChild ?? throw new NullReferenceException();
        _ = root.SecondChildren ?? throw new NullReferenceException();
        Assert.AreEqual("42", root.FirstChild.AttributeValue);
        Assert.AreEqual(2, root.SecondChildren.Count());
        var array = root.SecondChildren.Select(e => e.InnerText)
            .ToArray();
        var expect = new[] { "10", "20" };
        Assert.AreEqual(expect[0], array[0]);
        Assert.AreEqual(expect[1], array[1]);
        Assert.IsNull(root.ThirdChild);
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

        Assert.AreEqual("Hello", root.Foo);
        Assert.AreEqual("World", root.Bar);
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

        Assert.AreEqual("Hello, World", root.Foo);
        var list = root.Bar.ToList();
        Assert.AreEqual(2, list.Count);
        Assert.AreEqual("firstBar", list[0]);
        Assert.AreEqual("secondBar", list[1]);
        Assert.IsNull(root.Baz);
    }

    [ForElement("root")]
    public record class RequiredOptionalMultipleRoot(
        [Required] First FirstChild,
        [Optional] Second? SecondChild,
        [Multiple] IEnumerable<Third> ThirdChildren);

    [ForElement("root")]
    public record class RequiredMultipleOptionalRoot(
        [Required] First FirstChild,
        [Multiple] IEnumerable<Second> SecondChildren,
        [Optional] Third? ThirdChild);

    [ForElement("first")]
    public record class First(
        [ForAttribute("value")] string? AttributeValue);

    [ForElement("second")]
    public record class Second(
        [ForText] string InnerText);

    [ForElement("third")]
    public record class Third(
        [ForText] string InnerText);

    [ForElement("root")]
    public record class FooBarRoot(
        [Required][ForChildElement("foo")] string Foo,
        [Required][ForChildElement("bar")] string Bar);

    [ForElement("root")]
    public record class OptionalFooMultipleBarRoot(
        [Optional][ForChildElement("foo")] string? Foo,
        [Multiple][ForChildElement("bar")] IEnumerable<string> Bar,
        [Optional][ForChildElement("baz")] string? Baz);
}

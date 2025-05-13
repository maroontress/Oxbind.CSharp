namespace Maroontress.Oxbind.Test;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class EmptyElementTest
{
    [TestMethod]
    public void EmptyElement()
    {
        const string xml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <root/>
            """;
        var factory = new OxbinderFactory();
        var binder = factory.Of<Root>();
        var reader = new StringReader(xml);
        var root = binder.NewInstance(reader);

        Assert.IsNull(root.FirstChild);
        _ = root.SecondChildren ?? throw new NullReferenceException();
        Assert.AreEqual(0, root.SecondChildren.Count());
    }

    [TestMethod]
    public void EmptyTextElement()
    {
        const string xml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <root>
              <first/>
            </root>
            """;
        var factory = new OxbinderFactory();
        var binder = factory.Of<Root>();
        var reader = new StringReader(xml);
        var root = binder.NewInstance(reader);

        Assert.IsNotNull(root.FirstChild);
        _ = root.SecondChildren ?? throw new NullReferenceException();
        Assert.AreEqual("", root.FirstChild.InnerText);
        Assert.AreEqual(0, root.SecondChildren.Count());
    }

    [ForElement("root")]
    public record class Root(
        [Optional] First? FirstChild,
        [Multiple] IEnumerable<Second> SecondChildren);

    [ForElement("first")]
    public record class First(
        [ForText] string InnerText);

    [ForElement("second")]
    public record class Second(
        [ForText] string InnerText);
}

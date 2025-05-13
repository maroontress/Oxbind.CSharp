namespace Maroontress.Oxbind.Test.Oxbind.Impl.Metadata;

using System;
using System.IO;
using Maroontress.Oxbind;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class UnknownAttributeTest
{
    [TestMethod]
    public void RootTest()
    {
        const string xml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <root>
              <first fake="70" value="42" dummy="90"/>
            </root>
            """;
        var factory = new OxbinderFactory();
        var binder = factory.Of<Root>();
        var reader = new StringReader(xml);
        var root = binder.NewInstance(reader);

        var firstChild = root.FirstChild;
        _ = firstChild ?? throw new NullReferenceException();
        Assert.IsNotNull(firstChild.AttributeValue);
        Assert.AreEqual("42", firstChild.AttributeValue);
    }

    [ForElement("root")]
    public record class Root([Required] First FirstChild);

    [ForElement("first")]
    public record class First(
        [ForAttribute("value")] string? AttributeValue);
}

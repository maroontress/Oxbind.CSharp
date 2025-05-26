namespace Maroontress.Oxbind.Test;

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class NamespaceTest
{
    private const string AlphaNamespace
        = "http://matoontress.com/Oxbind/alpha";

    private const string BetaNamespace
        = "http://matoontress.com/Oxbind/beta";

    [TestMethod]
    public void DefaultNamespace()
    {
        var xml = $"""
            <?xml version="1.0" encoding="UTF-8"?>
            <root xmlns="{AlphaNamespace}" xmlns:b="{BetaNamespace}">
                <first b:value="10" value="20">42</first>
            </root>
            """;
        var factory = new OxbinderFactory();
        var binder = factory.Of<Root>();
        var reader = new StringReader(xml);
        var root = binder.NewInstance(reader);

        var firstChild = root.FirstChild;
        _ = firstChild ?? throw new NullReferenceException();
        Assert.IsNotNull(firstChild.AlphaValue);
        Assert.IsNotNull(firstChild.BetaValue);
        Assert.AreEqual("42", firstChild.InnerText);
        Assert.AreEqual("10", firstChild.BetaValue);
        Assert.AreEqual("20", firstChild.AlphaValue);
    }

    [ForElement("root", AlphaNamespace)]
    public record class Root(
        [Required] First FirstChild);

    [ForElement("first", AlphaNamespace)]
    public record class First(
        [ForAttribute("value", BetaNamespace)] string? BetaValue,
        [ForAttribute("value")] string? AlphaValue,
        [ForText] string InnerText);
}

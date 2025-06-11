namespace Maroontress.Oxbind.Test;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class PlaceholderTest
{
    [TestMethod]
    public void Breadth()
    {
        const string xml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <root>
              <item first="true" third="false" />
              <item second="true" fourth="false" />
            </root>
            """;
        var factory = new OxbinderFactory();
        var binder = factory.Of<Root>();
        var reader = new StringReader(xml);
        var root = binder.NewInstance(reader);

        var items = root.Children.ToList();
        Assert.AreEqual(2, items.Count);

        var firstChild = items[0];
        Assert.IsNotNull(firstChild);
        Assert.AreEqual("true", firstChild.First);
        Assert.IsNull(firstChild.Second);
        Assert.AreEqual("false", firstChild.Third);
        Assert.IsNull(firstChild.Fourth);
        Assert.IsEmpty(firstChild.Children);

        var secondChild = items[1];
        Assert.IsNotNull(secondChild);
        Assert.IsNull(secondChild.First);
        Assert.AreEqual("true", secondChild.Second);
        Assert.IsNull(secondChild.Third);
        Assert.AreEqual("false", secondChild.Fourth);
        Assert.IsEmpty(secondChild.Children);
    }

    [TestMethod]
    public void Depth()
    {
        const string xml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <root>
              <item first="true" third="false">
                <item second="true" fourth="false" />
              </item>
            </root>
            """;
        var factory = new OxbinderFactory();
        var binder = factory.Of<Root>();
        var reader = new StringReader(xml);
        var root = binder.NewInstance(reader);

        var items = root.Children.ToList();
        Assert.AreEqual(1, items.Count);

        var firstChild = items[0];
        Assert.IsNotNull(firstChild);
        Assert.AreEqual("true", firstChild.First);
        Assert.IsNull(firstChild.Second);
        Assert.AreEqual("false", firstChild.Third);
        Assert.IsNull(firstChild.Fourth);

        var nextItems = firstChild.Children.ToList();
        Assert.AreEqual(1, nextItems.Count);

        var secondChild = nextItems[0];
        Assert.IsNotNull(secondChild);
        Assert.IsNull(secondChild.First);
        Assert.AreEqual("true", secondChild.Second);
        Assert.IsNull(secondChild.Third);
        Assert.AreEqual("false", secondChild.Fourth);
        Assert.IsEmpty(secondChild.Children);
    }

    [ForElement("root")]
    public record class Root(
        [Multiple] IEnumerable<Item> Children);

    [ForElement("item")]
    public record class Item(
        [ForAttribute("first")] string? First,
        [ForAttribute("second")] string? Second,
        [ForAttribute("third")] string? Third,
        [ForAttribute("fourth")] string? Fourth,
        [Multiple] IEnumerable<Item> Children);
}

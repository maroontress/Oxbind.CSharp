namespace Maroontress.Oxbind.Test;

using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class TextTest
{
    [TestMethod]
    public void Empty()
    {
        const string xml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <root></root>
            """;
        var factory = new OxbinderFactory();
        var binder = factory.Of<Root>();
        var reader = new StringReader(xml);
        var root = binder.NewInstance(reader);

        Assert.AreEqual("", root.InnerText);
    }

    [TestMethod]
    public void Entity()
    {
        const string xml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <root>Hello&lt;&amp;&gt;World</root>
            """;
        var factory = new OxbinderFactory();
        var binder = factory.Of<Root>();
        var reader = new StringReader(xml);
        var root = binder.NewInstance(reader);

        Assert.AreEqual("Hello<&>World", root.InnerText);
    }

    [TestMethod]
    public void CDataText()
    {
        const string xml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <root>Hello<![CDATA[<&>]]>World</root>
            """;
        var factory = new OxbinderFactory();
        var binder = factory.Of<Root>();
        var reader = new StringReader(xml);
        var root = binder.NewInstance(reader);

        Assert.AreEqual("Hello<&>World", root.InnerText);
    }

    [TestMethod]
    public void Comment()
    {
        const string xml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <root>Hello<!-- -->World</root>
            """;
        var factory = new OxbinderFactory();
        var binder = factory.Of<Root>();
        var reader = new StringReader(xml);
        var root = binder.NewInstance(reader);

        Assert.AreEqual("HelloWorld", root.InnerText);
    }

    [ForElement("root")]
    public record class Root([ForText] string InnerText);
}

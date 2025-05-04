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
        const string xml = ""
            + "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n"
            + "<root>\r\n"
            + "  <first fake=\"70\" value=\"80\" dummy=\"90\"/>\r\n"
            + "</root>\r\n";

        var factory = new OxbinderFactory();
        var binder = factory.Of<Root>();
        var reader = new StringReader(xml);
        var root = binder.NewInstance(reader);

        _ = root.First ?? throw new NullReferenceException();
        Assert.AreEqual("80", root.First.Value);
    }

    [ForElement("root")]
    public sealed class Root
    {
        [ElementSchema]
        private static readonly Schema TheSchema = Schema.Of(
                Mandatory.Of<First>());

        [field: ForChild]
        public First? First { get; }
    }

    [ForElement("first")]
    public sealed class First
    {
        [field: ForAttribute("value")]
        public string? Value { get; }
    }
}

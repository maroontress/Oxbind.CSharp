namespace Maroontress.Oxbind.Test.Oxbind.Impl.Validator;

using System;
using Maroontress.Oxbind.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class ElementSchemaIgnoredTest
{
    [TestMethod]
    public void RootTest()
    {
        var v = new Validator(typeof(Root));
        Assert.AreEqual(
            "Root: Warning: [ElementSchema] is ignored for instance "
            + "fields: ignored",
            string.Join(Environment.NewLine, v.GetMessages()));
    }

    [ForElement("root")]
    public sealed class Root
    {
        [ElementSchema]
        private static readonly Schema TheSchema = Schema.Of(
                Mandatory.Of<First>());

        [ElementSchema]
        private readonly string ignored = nameof(ignored);

        [field: ForChild]
        private First? First { get; set; }
    }

    [ForElement("first")]
    public sealed class First
    {
        [field: ForAttribute("value")]
        public string? Value { get; }
    }
}

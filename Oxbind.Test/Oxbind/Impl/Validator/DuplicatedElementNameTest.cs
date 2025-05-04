namespace Maroontress.Oxbind.Test.Oxbind.Impl.Validator;

using System;
using Maroontress.Oxbind.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class DuplicatedElementNameTest
{
    [TestMethod]
    public void RootTest()
    {
        var v = new Validator(typeof(Root));
        Assert.AreEqual(
            "Root: Error: duplicated child element name: "
            + "'first' at First, Second",
            string.Join(Environment.NewLine, v.GetMessages()));
    }

    [ForElement("root")]
    public sealed class Root
    {
        [ElementSchema]
        private static readonly Schema TheSchema = Schema.Of(
                Mandatory.Of<First>(),
                Optional.Of<Second>());

        [field: ForChild]
        private First? First { get; set; }

        [field: ForChild]
        private Second? Second { get; set; }
    }

    [ForElement("first")]
    public sealed class First
    {
        [field: ForAttribute("value")]
        public string? Value { get; }
    }

    [ForElement("first")]
    public sealed class Second
    {
        [field: ForAttribute("value")]
        public string? Value { get; }
    }
}

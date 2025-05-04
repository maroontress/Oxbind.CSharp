namespace Maroontress.Oxbind.Test.Oxbind.Impl.Validator;

using System;
using Maroontress.Oxbind.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class NotHandledClassTest
{
    [TestMethod]
    public void RootTest()
    {
        var v = new Validator(typeof(Root));
        Assert.AreEqual(
            "Root: Error: the type in the Schema object is not handled "
            + "with [ForChild] or [FromChild]: IEnumerable<Third>, Second",
            string.Join(Environment.NewLine, v.GetMessages()));
    }

    [ForElement("root")]
    public sealed class Root
    {
        [ElementSchema]
        private static readonly Schema TheSchema = Schema.Of(
                Mandatory.Of<First>(),
                Optional.Of<Second>(),
                Multiple.Of<Third>());

        [field: ForChild]
        private First? First { get; set; }
    }

    [ForElement("first")]
    public sealed class First
    {
        [field: ForAttribute("value")]
        public string? Value { get; }
    }

    [ForElement("second")]
    public sealed class Second
    {
        [field: ForAttribute("value")]
        public string? Value { get; }
    }

    [ForElement("third")]
    public sealed class Third
    {
        [field: ForAttribute("value")]
        public string? Value { get; }
    }
}

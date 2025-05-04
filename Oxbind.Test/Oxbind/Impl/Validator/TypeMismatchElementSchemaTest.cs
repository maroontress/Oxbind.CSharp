namespace Maroontress.Oxbind.Test.Oxbind.Impl.Validator;

using System;
using Maroontress.Oxbind.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class TypeMismatchElementSchemaTest
{
    [TestMethod]
    public void RootTest()
    {
        var v = new Validator(typeof(Root));
        Assert.AreEqual(
            "Root: Error: the type of the field annotated with "
            + "[ElementSchema] is not Schema class: TheSchema",
            string.Join(Environment.NewLine, v.GetMessages()));
    }

    [ForElement("root")]
    public sealed class Root
    {
        [ElementSchema]
        private static readonly object TheSchema = Schema.Of(
                Mandatory.Of<First>());

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

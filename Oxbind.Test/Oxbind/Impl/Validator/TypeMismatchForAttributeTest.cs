namespace Maroontress.Oxbind.Test.Oxbind.Impl.Validator;

using System;
using Maroontress.Oxbind.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class TypeMismatchForAttributeTest
{
    [TestMethod]
    public void RootTest()
    {
        var logger = new Journal("Root");
        var v = Validators.New<Root>(logger);
        Assert.IsFalse(v.IsValid);
        Assert.AreEqual(
            """
            Root: Error: The type of a constructor parameter attributed with [ForAttribute] must be string? or BindResult<string>?: IntValue, ObjectValue.
            """,
            string.Join(Environment.NewLine, logger.GetMessages()));
    }

    [ForElement("root")]
    public record class Root(
        [ForAttribute("value")] object? ObjectValue,
        [ForAttribute("count")] int? IntValue);
}

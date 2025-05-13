namespace Maroontress.Oxbind.Test.Oxbind.Impl.Validator;

using System;
using Maroontress.Oxbind.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class DupliacatedAttributeNameTest
{
    [TestMethod]
    public void RootTest()
    {
        var logger = new Journal("Root");
        var v = new Validator(typeof(Root), logger);
        Assert.IsFalse(v.IsValid);
        Assert.AreEqual(
            """
            Root: Error: The attribute name 'value' is duplicated for [ForAttribute] attributes on constructor parameter(s): Another, Value.
            """,
            string.Join(Environment.NewLine, logger.GetMessages()));
    }

    [ForElement("root")]
    public record class Root(
        [ForAttribute("value")] string? Value,
        [ForAttribute("value")] string? Another);
}

namespace Maroontress.Oxbind.Test.Oxbind.Impl.Validator;

using System;
using Maroontress.Oxbind.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class TypeMismatchForTextTest
{
    [TestMethod]
    public void RootTest()
    {
        var logger = new Journal("Root");
        var v = Validators.New<Root>(logger);
        Assert.IsFalse(v.IsValid);
        Assert.AreEqual(
            """
            Root: Error: The type of a constructor parameter attributed with [ForText] must be string or BindResult<string>: InnerText.
            """,
            string.Join(Environment.NewLine, logger.GetMessages()));
    }

    [ForElement("root")]
    public record class Root(
        [ForText] object InnerText);
}

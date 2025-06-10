namespace Maroontress.Oxbind.Test.Oxbind.Impl.Validator;

using System;
using Maroontress.Oxbind.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class DuplicatedForTextTest
{
    [TestMethod]
    public void RootTest()
    {
        var logger = new Journal("Root");
        var v = Validators.New<Root>(logger);
        Assert.IsFalse(v.IsValid);
        Assert.AreEqual(
            """
            Root: Error: A constructor must not have two or more parameters attributed with [ForText]: Extra, InnerText.
            """,
            string.Join(Environment.NewLine, logger.GetMessages()));
    }

    [ForElement("root")]
    public record class Root(
        [ForText] string InnerText,
        [ForText] string Extra);
}

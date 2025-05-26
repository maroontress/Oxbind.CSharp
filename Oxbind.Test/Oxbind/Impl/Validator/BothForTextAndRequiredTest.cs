namespace Maroontress.Oxbind.Test.Oxbind.Impl.Validator;

using System;
using Maroontress.Oxbind.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class BothForTextAndRequiredTest
{
    [TestMethod]
    public void RootTest()
    {
        var logger = new Journal("Root");
        var v = new Validator(typeof(Root), logger);
        Assert.IsFalse(v.IsValid);
        Assert.AreEqual(
            """
            Root: Error: A constructor must not mix parameters attributed with [ForText] (InnerText) and parameters attributed with [Required], [Optional], or [Multiple] (FirstChild).
            """,
            string.Join(Environment.NewLine, logger.GetMessages()));
    }

    [ForElement("root")]
    public record class Root(
        [Required] First FirstChild,
        [ForText] string InnerText);

    [ForElement("first")]
    public record class First;
}

namespace Maroontress.Oxbind.Test.Oxbind.Impl.Validator;

using System;
using Maroontress.Oxbind.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class BothForTextAndRequiredTest
{
    [TestMethod]
    public void TwoParametersConflict()
    {
        var logger = new Journal("Root");
        var v = new Validator(typeof(Root1), logger);
        Assert.IsFalse(v.IsValid);
        Assert.AreEqual(
            """
            Root: Error: A constructor must not mix parameters attributed with [ForText] (InnerText) and parameters attributed with [Required], [Optional], or [Multiple] (FirstChild).
            """,
            string.Join(Environment.NewLine, logger.GetMessages()));
    }

    [TestMethod]
    public void OneParameterHasForTextAndRequired()
    {
        var logger = new Journal("Root");
        var v = new Validator(typeof(Root2), logger);
        Assert.IsFalse(v.IsValid);
        Assert.AreEqual(
            """
            Root: Error: A constructor parameter must not be attributed with both [ForText] and another Oxbind binding attribute (e.g., [Required]): FirstChild.
            Root: Error: The type of a constructor parameter attributed with [ForText] must be string or BindResult<string>: FirstChild.
            """,
            string.Join(Environment.NewLine, logger.GetMessages()));
    }

    [ForElement("root")]
    public record class Root1(
        [Required] First FirstChild,
        [ForText] string InnerText);

    [ForElement("root")]
    public record class Root2(
        [Required][ForText] First FirstChild);

    [ForElement("first")]
    public record class First;
}

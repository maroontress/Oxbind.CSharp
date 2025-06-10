namespace Maroontress.Oxbind.Test.Oxbind.Impl.Validator;

using System;
using Maroontress.Oxbind.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class AttributeAfterElementTest
{
    [TestMethod]
    public void RootTest()
    {
        var logger = new Journal("Root");
        var v = Validators.New<Root>(logger);
        Assert.IsFalse(v.IsValid);
        Assert.AreEqual(
            """
            Root: Error: Constructor parameters attributed with [ForAttribute] must appear consecutively at the beginning of the parameter list. Misplaced parameter(s): Language, Year.
            """,
            string.Join(Environment.NewLine, logger.GetMessages()));
    }

    [ForElement("root")]
    public record class Root(
        [ForAttribute("title")] string? Talue,
        [ForText] string InnerText,
        [ForAttribute("year")] string? Year,
        [ForAttribute("language")] string? Language);
}

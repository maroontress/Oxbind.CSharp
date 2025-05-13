namespace Maroontress.Oxbind.Test.Oxbind.Impl.Validator;

using System;
using System.Collections.Generic;
using Maroontress.Oxbind.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class NotAnnotatedWithForElementTest
{
    [TestMethod]
    public void RootTest()
    {
        var logger = new Journal("Root");
        var v = new Validator(typeof(Root), logger);
        Assert.IsFalse(v.IsValid);
        Assert.AreEqual(
            """
            Root: Error: The type of constructor parameter(s) FirstChild, SecondChildren, ThirdChild must be a class attributed with [ForElement].
            """,
            string.Join(Environment.NewLine, logger.GetMessages()));
    }

    [ForElement("root")]
    public record class Root(
        [Required] First FirstChild,
        [Multiple] IEnumerable<Second> SecondChildren,
        [Optional] Third? ThirdChild);

    /* No [ForElement] */
    public sealed class First;

    /* No [ForElement] */
    public sealed class Second;

    /* No [ForElement] */
    public sealed class Third;
}

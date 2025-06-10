namespace Maroontress.Oxbind.Test.Oxbind.Impl.Validator;

using System;
using Maroontress.Oxbind.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class NoForElementTest
{
    [TestMethod]
    public void RootTest()
    {
        var logger = new Journal("Root");
        var v = Validators.New<Root>(logger);
        Assert.IsFalse(v.IsValid);
        Assert.AreEqual(
            """
            Root: Error: The class must be attributed with [ForElement].
            """,
            string.Join(Environment.NewLine, logger.GetMessages()));
    }

    public sealed class Root;
}

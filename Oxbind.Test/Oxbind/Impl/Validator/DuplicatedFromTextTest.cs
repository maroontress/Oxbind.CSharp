namespace Maroontress.Oxbind.Test.Oxbind.Impl.Validator;

using System;
using Maroontress.Oxbind.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StyleChecker.Annotations;

[TestClass]
public sealed class DuplicatedFromTextTest
{
    [TestMethod]
    public void RootTest()
    {
        var v = new Validator(typeof(Root));
        Assert.AreEqual(
            "Root: Error: must not have two or more [FromText]s: "
            + "Text(String), Value(String)",
            string.Join(Environment.NewLine, v.GetMessages()));
    }

    [ForElement("root")]
    public sealed class Root
    {
        [FromText]
        private void Text([Unused] string value)
        {
        }

        [FromText]
        private void Value([Unused] string value)
        {
        }
    }
}

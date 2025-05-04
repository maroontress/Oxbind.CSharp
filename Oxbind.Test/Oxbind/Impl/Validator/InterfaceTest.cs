namespace Maroontress.Oxbind.Test.Oxbind.Impl.Validator;

using System;
using Maroontress.Oxbind.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class InterfaceTest
{
    public interface IRoot
    {
    }

    [TestMethod]
    public void RootTest()
    {
        var v = new Validator(typeof(IRoot));
        Assert.AreEqual(
            "IRoot: Error: must be annotated with [ForElement]"
            + Environment.NewLine
            + "IRoot: Error: must not be interface",
            string.Join(Environment.NewLine, v.GetMessages()));
    }

    [ForElement("root")]
    public sealed class Root : IRoot
    {
    }
}

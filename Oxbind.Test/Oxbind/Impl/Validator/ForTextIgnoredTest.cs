namespace Maroontress.Oxbind.Impl.Validator.Test;

using System;
using Maroontress.Oxbind.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class ForTextIgnoredTest
{
    [TestMethod]
    public void RootTest()
    {
        var v = new Validator(typeof(Root));
        Assert.AreEqual(
            "Root: Warning: [ForText] is ignored for static fields: Text",
            string.Join(Environment.NewLine, v.GetMessages()));
    }

    [ForElement("root")]
    public sealed class Root
    {
        [ForText]
        private static readonly string Text = nameof(Text);
    }
}

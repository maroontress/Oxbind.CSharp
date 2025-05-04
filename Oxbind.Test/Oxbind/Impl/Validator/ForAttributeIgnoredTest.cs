namespace Maroontress.Oxbind.Test.Oxbind.Impl.Validator;

using System;
using Maroontress.Oxbind.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class ForAttributeIgnoredTest
{
    [TestMethod]
    public void RootTest()
    {
        var v = new Validator(typeof(Root));
        Assert.AreEqual(
            "Root: Warning: [ForAttribute] is ignored for static fields: "
            + "<Text>k__BackingField",
            string.Join(Environment.NewLine, v.GetMessages()));
    }

    [ForElement("root")]
    public sealed class Root
    {
        [field: ForAttribute("value")]
        public static string? Text { get; set; }
    }
}

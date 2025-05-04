namespace Maroontress.Oxbind.Test.Oxbind.Impl.Validator;

using System;
using Maroontress.Oxbind.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class TypeMismatchForTextTest
{
    [TestMethod]
    public void RootTest()
    {
        var v = new Validator(typeof(Root));
        Assert.AreEqual(
            "Root: Error: the type of the field annotated with "
            + "[ForText] is not string: <Text>k__BackingField",
            string.Join(Environment.NewLine, v.GetMessages()));
    }

    [ForElement("root")]
    public sealed class Root
    {
        [field: ForText]
        private int Text { get; set; }
    }
}

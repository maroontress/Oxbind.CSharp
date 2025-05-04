namespace Maroontress.Oxbind.Test.Oxbind.Impl.Validator;

using System;
using System.Collections.Generic;
using Maroontress.Oxbind.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class UnusedForChildFromChildTest
{
    [TestMethod]
    public void RootTest()
    {
        var v = new Validator(typeof(Root));
        Assert.AreEqual(
            "Root: Warning: unused [ForChild] or [FromChild] attributes: "
            + "IEnumerable<Third> at <ThirdCombo>k__BackingField",
            string.Join(Environment.NewLine, v.GetMessages()));
    }

    [ForElement("root")]
    public sealed class Root
    {
        [ElementSchema]
        private static readonly Schema TheSchema = Schema.Of(
                Mandatory.Of<First>(),
                Optional.Of<Second>());

        [field: ForChild]
        private First? First { get; set; }

        [field: ForChild]
        private Second? Second { get; set; }

        [field: ForChild]
        private IEnumerable<Third>? ThirdCombo { get; set; }
    }

    [ForElement("first")]
    public sealed class First
    {
        [field: ForAttribute("value")]
        public string? Value { get; }
    }

    [ForElement("second")]
    public sealed class Second
    {
        [field: ForAttribute("value")]
        public string? Value { get; }
    }

    [ForElement("third")]
    public sealed class Third
    {
        [field: ForAttribute("value")]
        public string? Value { get; }
    }
}

#pragma warning disable CS1591

namespace Maroontress.Oxbind.Impl.Validator.Test
{
    using System;
    using Maroontress.Oxbind.Impl;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StyleChecker.Annotations;

    [TestClass]
    public sealed class FromChildIgnoredTest
    {
        [TestMethod]
        public void RootTest()
        {
            var v = new Validator(typeof(Root));
            Assert.AreEqual(
                "Root: Warning: [FromChild] is ignored for static fields: "
                + "Ignored(First)",
                string.Join(Environment.NewLine, v.GetMessages()));
        }

        [ForElement("root")]
        public sealed class Root
        {
            [ElementSchema]
            private static readonly Schema TheSchema = Schema.Of(
                    Mandatory.Of<First>());

            [field: ForChild]
            private First First { get; set; }

            [FromChild]
            private static void Ignored([Unused] First first)
            {
            }
        }

        [ForElement("first")]
        public sealed class First
        {
            [field: ForAttribute("value")]
            public string Value { get; }
        }
    }
}

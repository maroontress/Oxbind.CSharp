#pragma warning disable CS1591

namespace Maroontress.Oxbind.Impl.Validator.Test
{
    using System;
    using Maroontress.Oxbind.Impl;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class ForChildIgnoredTest
    {
        [TestMethod]
        public void RootTest()
        {
            var v = new Validator(typeof(Root));
            Assert.AreEqual(
                "Root: Warning: [ForChild] is ignored for static fields: "
                + "Ignored",
                string.Join(Environment.NewLine, v.GetMessages()));
        }

        [ForElement("root")]
        public sealed class Root
        {
            [ElementSchema]
            private static readonly Schema TheSchema = Schema.Of(
                    Mandatory.Of<First>());

            [ForChild]
            private static readonly string Ignored = nameof(Ignored);

            [field: ForChild]
            private First? First { get; set; }
        }

        [ForElement("first")]
        public sealed class First
        {
            [field: ForAttribute("value")]
            public string? Value { get; }
        }
    }
}

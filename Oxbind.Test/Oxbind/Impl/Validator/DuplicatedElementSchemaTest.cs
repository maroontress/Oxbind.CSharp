#pragma warning disable CS1591

namespace Maroontress.Oxbind.Impl.Validator.Test
{
    using System;
    using Maroontress.Oxbind.Impl;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class DuplicatedElementSchemaTest
    {
        [TestMethod]
        public void RootTest()
        {
            var v = new Validator(typeof(Root));
            Assert.AreEqual(
                "Root: Error: must not have two or more [ElementSchema]s: "
                + "TheOtherSchema, TheSchema",
                string.Join(Environment.NewLine, v.GetMessages()));
        }

        [ForElement("root")]
        public sealed class Root
        {
            [ElementSchema]
            private static readonly Schema TheSchema = Schema.Of(
                    Mandatory.Of<First>());

            [ElementSchema]
            private static readonly Schema TheOtherSchema = Schema.Of(
                    Optional.Of<First>());

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

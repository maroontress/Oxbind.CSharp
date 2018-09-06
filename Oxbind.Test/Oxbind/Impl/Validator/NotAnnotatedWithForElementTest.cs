#pragma warning disable CS1591

namespace Maroontress.Oxbind.Impl.Validator.Test
{
    using System;
    using Maroontress.Oxbind.Impl;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class NotAnnotatedWithForElementTest
    {
        [TestMethod]
        public void RootTest()
        {
            var v = new Validator(typeof(Root));
            Assert.AreEqual(
                "Root: Error: the type that the Schema object contains "
                + "is not the class annotated with [ForElement]: First",
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
        }

        public sealed class First
        {
            [field: ForAttribute("value")]
            public string Value { get; }
        }
    }
}

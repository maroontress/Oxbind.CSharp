#pragma warning disable CS1591

namespace Maroontress.Oxbind.Impl.Validator.Test
{
    using System;
    using Maroontress.Oxbind.Impl;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StyleChecker.Annotations;

    [TestClass]
    public sealed class DuplicatedChildClassTest
    {
        [TestMethod]
        public void FieldAndMethodRootTest()
        {
            var v = new Validator(typeof(FieldAndMethodRoot));
            Assert.AreEqual(
                "FieldAndMethodRoot: Error: duplicated child element "
                + "class: First at <First>k__BackingField, "
                + "Another(First)",
                string.Join(Environment.NewLine, v.GetMessages()));
        }

        [TestMethod]
        public void FieldRootTest()
        {
            var v = new Validator(typeof(FieldRoot));
            Assert.AreEqual(
                "FieldRoot: Error: duplicated child element class: First "
                + "at <Another>k__BackingField, <First>k__BackingField",
                string.Join(Environment.NewLine, v.GetMessages()));
        }

        [ForElement("root")]
        public sealed class FieldAndMethodRoot
        {
            [ElementSchema]
            private static readonly Schema TheSchema = Schema.Of(
                    Mandatory.Of<First>());

            [field: ForChild]
            private First? First { get; set; }

            [FromChild]
            private void Another([Unused] First first)
            {
            }
        }

        [ForElement("root")]
        public sealed class FieldRoot
        {
            [ElementSchema]
            private static readonly Schema TheSchema = Schema.Of(
                    Mandatory.Of<First>());

            [field: ForChild]
            private First? First { get; set; }

            [field: ForChild]
            private First? Another { get; set; }
        }

        [ForElement("first")]
        public sealed class First
        {
            [field: ForAttribute("value")]
            public string? Value { get; }
        }
    }
}

#pragma warning disable CS1591

namespace Maroontress.Oxbind.Impl.Validator.Test
{
    using System;
    using Maroontress.Oxbind.Impl;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StyleChecker.Annotations;

    [TestClass]
    public sealed class DupliacatedAttributeNameTest
    {
        [TestMethod]
        public void FieldRootTest()
        {
            var v = new Validator(typeof(FieldRoot));
            Assert.AreEqual(
                "FieldRoot: Error: duplicated attribute name: 'value' "
                + "at <Another>k__BackingField, <Value>k__BackingField",
                string.Join(Environment.NewLine, v.GetMessages()));
        }

        [TestMethod]
        public void FieldAndMethodRootTest()
        {
            var v = new Validator(typeof(FieldAndMethodRoot));
            Assert.AreEqual(
                "FieldAndMethodRoot: Error: duplicated attribute name: "
                + "'value' at <Value>k__BackingField, Another(String)",
                string.Join(Environment.NewLine, v.GetMessages()));
        }

        [ForElement("root")]
        public sealed class FieldRoot
        {
            [field: ForAttribute("value")]
            private string Value { get; set; }

            [field: ForAttribute("value")]
            private string Another { get; set; }
        }

        [ForElement("root")]
        public sealed class FieldAndMethodRoot
        {
            [field: ForAttribute("value")]
            private string Value { get; set; }

            [FromAttribute("value")]
            private void Another([Unused] string value)
            {
            }
        }
    }
}

#pragma warning disable CS1591

namespace Maroontress.Oxbind.Impl.Validator.Test
{
    using System;
    using Maroontress.Oxbind.Impl;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StyleChecker.Annotations;

    [TestClass]
    public sealed class BothFromTextAndElementSchemaTest
    {
        [TestMethod]
        public void RootTest()
        {
            var v = new Validator(typeof(Root));
            Assert.AreEqual(
                "Root: Error: [ElementSchema] must not be combined with "
                + "[FromText]: Text(String) and TheSchema",
                string.Join(Environment.NewLine, v.GetMessages()));
        }

        [ForElement("root")]
        public sealed class Root
        {
            [ElementSchema]
            private static readonly Schema TheSchema = Schema.Of();

            [FromText]
            private void Text([Unused] string value)
            {
            }
        }
    }
}

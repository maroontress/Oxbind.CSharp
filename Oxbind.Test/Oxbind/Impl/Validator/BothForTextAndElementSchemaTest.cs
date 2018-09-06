#pragma warning disable CS1591

namespace Maroontress.Oxbind.Impl.Validator.Test
{
    using System;
    using Maroontress.Oxbind.Impl;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class BothForTextAndElementSchemaTest
    {
        [TestMethod]
        public void RootTest()
        {
            var v = new Validator(typeof(Root));
            Assert.AreEqual(
                "Root: Error: [ElementSchema] must not be combined with "
                + "[ForText]: <Text>k__BackingField and TheSchema",
                string.Join(Environment.NewLine, v.GetMessages()));
        }

        [ForElement("root")]
        public sealed class Root
        {
            [ElementSchema]
            private static readonly Schema TheSchema = Schema.Of();

            [field: ForText]
            private string Text { get; set; }
        }
    }
}

#pragma warning disable CS1591

namespace Maroontress.Oxbind.Impl.Validator.Test
{
    using System;
    using Maroontress.Oxbind.Impl;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StyleChecker.Annotations;

    [TestClass]
    public sealed class BothForTextAndFromTextTest
    {
        [TestMethod]
        public void RootTest()
        {
            var v = new Validator(typeof(Root));
            Assert.AreEqual(
                "Root: Error: [ForText] must not be combined with "
                + "[FromText]: <Text>k__BackingField and Notify(String)",
                string.Join(Environment.NewLine, v.GetMessages()));
        }

        [ForElement("root")]
        public sealed class Root
        {
            [field: ForText]
            private string Text { get; set; }

            [FromText]
            private void Notify([Unused] string value)
            {
            }
        }
    }
}

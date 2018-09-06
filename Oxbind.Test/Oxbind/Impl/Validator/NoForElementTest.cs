#pragma warning disable CS1591

namespace Maroontress.Oxbind.Impl.Validator.Test
{
    using System;
    using Maroontress.Oxbind.Impl;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class NoForElementTest
    {
        [TestMethod]
        public void RootTest()
        {
            var v = new Validator(typeof(Root));
            Assert.AreEqual(
                "Root: Error: must be annotated with [ForElement]",
                string.Join(Environment.NewLine, v.GetMessages()));
        }

        public sealed class Root
        {
        }
    }
}

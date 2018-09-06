#pragma warning disable CS1591

namespace Maroontress.Oxbind.Impl.Validator.Test
{
    using System;
    using Maroontress.Oxbind.Impl;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StyleChecker.Annotations;

    [TestClass]
    public sealed class FromAttributeIgnoredTest
    {
        [TestMethod]
        public void RootTest()
        {
            var v = new Validator(typeof(Root));
            Assert.AreEqual(
                "Root: Warning: [FromAttribute] is ignored for static "
                + "methods: Ignored(String)",
                string.Join(Environment.NewLine, v.GetMessages()));
        }

        [ForElement("root")]
        public sealed class Root
        {
            [FromAttribute("value")]
            private static void Ignored([Unused] string value)
            {
            }
        }
    }
}

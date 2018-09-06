#pragma warning disable CS1591

namespace Maroontress.Oxbind.Impl.Validator.Test
{
    using System;
    using Maroontress.Oxbind.Impl;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class FromTextIgnoredTest
    {
        [TestMethod]
        public void RootTest()
        {
            var v = new Validator(typeof(Root));
            Assert.AreEqual(
                "Root: Warning: [FromText] is ignored for static methods: "
                + "Text()",
                string.Join(Environment.NewLine, v.GetMessages()));
        }

        [ForElement("root")]
        public sealed class Root
        {
            [FromText]
            private static string Text() => nameof(Text);
        }
    }
}

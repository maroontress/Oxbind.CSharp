namespace Maroontress.Oxbind.Test
{
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class NamespaceTest
    {
        private const string AlphaNamespace
            = "http://matoontress.com/Oxbind/alpha";

        private const string BetaNamespace
            = "http://matoontress.com/Oxbind/beta";

        [TestMethod]
        public void DefaultNamespace()
        {
            var xml = ""
                + $"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n"
                + $"<root xmlns=\"{AlphaNamespace}\"\r\n"
                + $"      xmlns:b=\"{BetaNamespace}\">\r\n"
                + $"  <first b:value=\"10\" value=\"30\">20</first>\r\n"
                + $"</root>\r\n";
            var factory = new OxbinderFactory();
            var binder = factory.Of<Root>();
            var reader = new StringReader(xml);
            var root = binder.NewInstance(reader);

            Assert.AreEqual("20", root.First.Text);
            Assert.AreEqual("10", root.First.BetaValue);
            Assert.AreEqual("30", root.First.Value);
        }

        [ForElement("root", AlphaNamespace)]
        public sealed class Root
        {
            [ElementSchema]
            private static readonly Schema TheSchema = Schema.Of(
                    Optional.Of<First>());

            [field: ForChild]
            public First First { get; }
        }

        [ForElement("first", AlphaNamespace)]
        public sealed class First
        {
            [field: ForAttribute("value", BetaNamespace)]
            public string BetaValue { get; }

            [field: ForAttribute("value")]
            public string Value { get; }

            [field: ForText]
            public string Text { get; }
        }
    }
}

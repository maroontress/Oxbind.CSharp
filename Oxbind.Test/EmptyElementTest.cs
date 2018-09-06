#pragma warning disable CS1591

namespace Maroontress.Oxbind.Test
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class EmptyElementTest
    {
        [TestMethod]
        public void EmptyElement()
        {
            const string xml = ""
                + "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n"
                + "<root/>\r\n";
            var factory = new OxbinderFactory();
            var binder = factory.Of<Root>();
            var reader = new StringReader(xml);
            var root = binder.NewInstance(reader);

            Assert.IsNull(root.First);
            Assert.AreEqual(0, root.SecondCombo.Count());
        }

        [TestMethod]
        public void EmptyTextElement()
        {
            const string xml = ""
                + "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n"
                + "<root>\r\n"
                + "  <first/>\r\n"
                + "</root>\r\n";
            var factory = new OxbinderFactory();
            var binder = factory.Of<Root>();
            var reader = new StringReader(xml);
            var root = binder.NewInstance(reader);

            Assert.AreEqual("", root.First.Value);
            Assert.AreEqual(0, root.SecondCombo.Count());
        }

        [ForElement("root")]
        public sealed class Root
        {
            [ElementSchema]
            private static readonly Schema TheSchema = Schema.Of(
                    Optional.Of<First>(),
                    Multiple.Of<Second>());

            [field: ForChild]
            public First First { get; }

            [field: ForChild]
            public IEnumerable<Second> SecondCombo { get; }
        }

        [ForElement("first")]
        public sealed class First
        {
            [field: ForText]
            public string Value { get; }
        }

        [ForElement("second")]
        public sealed class Second
        {
            [field: ForText]
            public string Value { get; }
        }
    }
}

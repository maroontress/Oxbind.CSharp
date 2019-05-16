namespace Maroontress.Oxbind.Test
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class BasicFieldTest
    {
        [TestMethod]
        public void MandatoryOptionalOneMultiple()
        {
            const string xml = ""
                + "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n"
                + "<root>\r\n"
                + "  <first value=\"80\"/>\r\n"
                + "  <second>text</second>\r\n"
                + "  <third>10</third>\r\n"
                + "  <third>20</third>\r\n"
                + "</root>\r\n";
            var factory = new OxbinderFactory();
            var binder = factory.Of<MandatoryOptionalMultipleRoot>();
            var reader = new StringReader(xml);
            var root = binder.NewInstance(reader);

            Assert.AreEqual("80", root.First.Value);
            Assert.AreEqual("text", root.Second.Value);
            Assert.AreEqual(2, root.ThirdCombo.Count());
            var array = root.ThirdCombo.Select(e => e.Value)
                .ToArray();
            var expect = new[] { "10", "20" };
            Assert.AreEqual(expect[0], array[0]);
            Assert.AreEqual(expect[1], array[1]);
        }

        [TestMethod]
        public void MandatoryOptionalZeroMultiple()
        {
            const string xml = ""
                + "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n"
                + "<root>\r\n"
                + "  <first value=\"80\"/>\r\n"
                + "  <third>10</third>\r\n"
                + "  <third>20</third>\r\n"
                + "</root>\r\n";
            var factory = new OxbinderFactory();
            var binder = factory.Of<MandatoryOptionalMultipleRoot>();
            var reader = new StringReader(xml);
            var root = binder.NewInstance(reader);

            Assert.AreEqual("80", root.First.Value);
            Assert.IsNull(root.Second);
            Assert.AreEqual(2, root.ThirdCombo.Count());
            var array = root.ThirdCombo.Select(e => e.Value)
                .ToArray();
            var expect = new[] { "10", "20" };
            Assert.AreEqual(expect[0], array[0]);
            Assert.AreEqual(expect[1], array[1]);
        }

        [TestMethod]
        public void MandatoryMultipleOptionalOne()
        {
            const string xml = ""
                + "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n"
                + "<root>\r\n"
                + "  <first value=\"80\"/>\r\n"
                + "  <second>10</second>\r\n"
                + "  <second>20</second>\r\n"
                + "  <third>text</third>\r\n"
                + "</root>\r\n";
            var factory = new OxbinderFactory();
            var binder = factory.Of<MandatoryMultipleOptionalRoot>();
            var reader = new StringReader(xml);
            var root = binder.NewInstance(reader);

            Assert.AreEqual("80", root.First.Value);
            Assert.AreEqual(2, root.SecondCombo.Count());
            var array = root.SecondCombo.Select(e => e.Value)
                .ToArray();
            var expect = new[] { "10", "20" };
            Assert.AreEqual(expect[0], array[0]);
            Assert.AreEqual(expect[1], array[1]);
            Assert.AreEqual("text", root.Third.Value);
        }

        [TestMethod]
        public void MandatoryMultipleOptionalZero()
        {
            const string xml = ""
                + "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n"
                + "<root>\r\n"
                + "  <first value=\"80\"/>\r\n"
                + "  <second>10</second>\r\n"
                + "  <second>20</second>\r\n"
                + "</root>\r\n";
            var factory = new OxbinderFactory();
            var binder = factory.Of<MandatoryMultipleOptionalRoot>();
            var reader = new StringReader(xml);
            var root = binder.NewInstance(reader);

            Assert.AreEqual("80", root.First.Value);
            Assert.AreEqual(2, root.SecondCombo.Count());
            var array = root.SecondCombo.Select(e => e.Value)
                .ToArray();
            var expect = new[] { "10", "20" };
            Assert.AreEqual(expect[0], array[0]);
            Assert.AreEqual(expect[1], array[1]);
            Assert.IsNull(root.Third);
        }

        [ForElement("root")]
        public sealed class MandatoryOptionalMultipleRoot
        {
            [ElementSchema]
            private static readonly Schema TheSchema = Schema.Of(
                    Mandatory.Of<First>(),
                    Optional.Of<Second>(),
                    Multiple.Of<Third>());

            [field: ForChild]
            public First First { get; }

            [field: ForChild]
            public Second Second { get; }

            [field: ForChild]
            public IEnumerable<Third> ThirdCombo { get; }
        }

        [ForElement("root")]
        public sealed class MandatoryMultipleOptionalRoot
        {
            [ElementSchema]
            private static readonly Schema TheSchema = Schema.Of(
                    Mandatory.Of<First>(),
                    Multiple.Of<Second>(),
                    Optional.Of<Third>());

            [field: ForChild]
            public First First { get; }

            [field: ForChild]
            public IEnumerable<Second> SecondCombo { get; }

            [field: ForChild]
            public Third Third { get; }
        }

        [ForElement("first")]
        public sealed class First
        {
            [field: ForAttribute("value")]
            public string Value { get; }
        }

        [ForElement("second")]
        public sealed class Second
        {
            [field: ForText]
            public string Value { get; }
        }

        [ForElement("third")]
        public sealed class Third
        {
            [field: ForText]
            public string Value { get; }
        }
    }
}

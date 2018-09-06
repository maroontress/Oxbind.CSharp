#pragma warning disable CS1591

namespace Maroontress.Oxbind.Test
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class BasicMethodTest
    {
        [TestMethod]
        public void RootTest()
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
            var binder = factory.Of<Root>();
            var reader = new StringReader(xml);
            var root = binder.NewInstance(reader);
            root.Test();
        }

        [ForElement("root")]
        public sealed class Root
        {
            [ElementSchema]
            private static readonly Schema TheSchema = Schema.Of(
                    Mandatory.Of<First>(),
                    Optional.Of<Second>(),
                    Multiple.Of<Third>());

            private First first;
            private Second second;
            private IEnumerable<Third> thirdCombo;

            [FromChild]
            public void Notify(First first)
            {
                this.first = first;
            }

            [FromChild]
            public void Notify(Second second)
            {
                this.second = second;
            }

            [FromChild]
            public void Notify(IEnumerable<Third> thirdCollection)
            {
                thirdCombo = thirdCollection;
            }

            public void Test()
            {
                Assert.AreEqual("80", first.Value);
                Assert.AreEqual("text", second.Value);
                Assert.AreEqual(2, thirdCombo.Count());
                var array = thirdCombo.Select(e => e.Value)
                    .ToArray();
                var expect = new[] { "10", "20" };
                Assert.AreEqual(expect[0], array[0]);
                Assert.AreEqual(expect[1], array[1]);
            }
        }

        [ForElement("first")]
        public sealed class First
        {
            public string Value { get; private set; }

            [FromAttribute("value")]
            public void Notify(string value) => Value = value;
        }

        [ForElement("second")]
        public sealed class Second
        {
            public string Value { get; private set; }

            [FromText]
            public void Notify(string value) => Value = value;
        }

        [ForElement("third")]
        public sealed class Third
        {
            public string Value { get; private set; }

            [FromText]
            public void Notify(string value) => Value = value;
        }
    }
}

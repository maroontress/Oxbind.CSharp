namespace Maroontress.Oxbind.Test
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class BasicEventMethodTest
    {
        [TestMethod]
        public void RootTest()
        {
            const string xml = ""
                + "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n"
                + "<root>\r\n"
                + "  <first value=\"80\"/>\r\n"
                + "  <second>10</second>\r\n"
                + "  <second>20</second>\r\n"
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
                    Multiple.Of<Second>());

            private BindEvent<First> first;
            private IEnumerable<BindEvent<Second>> secondCombo;

            [FromChild]
            public void Notify(BindEvent<First> first)
            {
                this.first = first;
            }

            [FromChild]
            public void Notify(IEnumerable<BindEvent<Second>> secondCombo)
            {
                this.secondCombo = secondCombo;
            }

            public void Test()
            {
                var firstValue = first.Value;
                Assert.AreEqual(3, first.Line);
                Assert.AreEqual(4, first.Column);
                var first80Value = firstValue.Value;
                Assert.AreEqual("80", first80Value.Value);
                Assert.AreEqual(3, first80Value.Line);
                Assert.AreEqual(10, first80Value.Column);

                var array = secondCombo.ToArray();
                Assert.AreEqual(2, array.Length);
                var second10 = array[0];
                var second10Value = second10.Value;
                Assert.AreEqual(4, second10.Line);
                Assert.AreEqual(4, second10.Column);
                Assert.AreEqual("10", second10Value.Value.Value);
                var second20 = array[1];
                var second20Value = second20.Value;
                Assert.AreEqual(5, second20.Line);
                Assert.AreEqual(4, second20.Column);
                Assert.AreEqual("20", second20Value.Value.Value);
            }
        }

        [ForElement("first")]
        public sealed class First
        {
            public BindEvent<string> Value { get; private set; }

            [FromAttribute("value")]
            public void Notify(BindEvent<string> value) => Value = value;
        }

        [ForElement("second")]
        public sealed class Second
        {
            public BindEvent<string> Value { get; private set; }

            [FromText]
            public void Notify(BindEvent<string> value) => Value = value;
        }
    }
}

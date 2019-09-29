namespace Maroontress.Oxbind.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class BasicEventFieldTest
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
            var binder = factory.Of<AttributeRoot>();
            var reader = new StringReader(xml);
            var root = binder.NewInstance(reader);
            root.Test();
        }

        [ForElement("root")]
        public sealed class AttributeRoot
        {
            [ElementSchema]
            private static readonly Schema TheSchema = Schema.Of(
                Mandatory.Of<First>(),
                Multiple.Of<Second>());

            [field: ForChild]
            private BindEvent<First>? First { get; }

            [field: ForChild]
            private IEnumerable<BindEvent<Second>>? SecondCombo { get; }

            public void Test()
            {
                _ = First ?? throw new NullReferenceException();
                var first = First.Value;
                Assert.AreEqual(3, First.Line);
                Assert.AreEqual(4, First.Column);
                var firstValue = first.Value
                    ?? throw new NullReferenceException();
                Assert.AreEqual("80", firstValue.Value);
                Assert.AreEqual(3, firstValue.Line);
                Assert.AreEqual(10, firstValue.Column);

                var array = SecondCombo.ToArray();
                Assert.AreEqual(2, array.Length);
                var second10 = array[0];
                var second10Value = second10.Value;
                Assert.AreEqual(4, second10.Line);
                Assert.AreEqual(4, second10.Column);
                Assert.AreEqual("10", second10Value.Value);
                var second20 = array[1];
                var second20Value = second20.Value;
                Assert.AreEqual(5, second20.Line);
                Assert.AreEqual(4, second20.Column);
                Assert.AreEqual("20", second20Value.Value);
            }
        }

        [ForElement("first")]
        public sealed class First
        {
            [field: ForAttribute("value")]
            public BindEvent<string>? Value { get; }
        }

        [ForElement("second")]
        public sealed class Second
        {
            [field: ForText]
            public string? Value { get; }
        }
    }
}

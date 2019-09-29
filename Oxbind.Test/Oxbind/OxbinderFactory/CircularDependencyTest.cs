#pragma warning disable CS1591

namespace Maroontress.Oxbind.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class CircularDependencyTest
    {
        [TestMethod]
        public void RootTest()
        {
            const string xml = ""
                + "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n"
                + "<root>\r\n"
                + "  <first value=\"80\"/>\r\n"
                + "  <second>text</second>\r\n"
                + "</root>\r\n";
            const string m = "Root has circular dependency.";
            Checks.ThrowBindException<Root>(xml, m);
        }

        [ForElement("root")]
        public sealed class Root
        {
            [ElementSchema]
            private static readonly Schema TheSchema = Schema.Of(
                    Mandatory.Of<First>());

            [field: ForChild]
            private First? First { get; }
        }

        [ForElement("first")]
        public sealed class First
        {
            [ElementSchema]
            private static readonly Schema TheSchema = Schema.Of(
                    Mandatory.Of<Second>());

            [field: ForChild]
            private Second? Second { get; }
        }

        [ForElement("second")]
        public sealed class Second
        {
            [ElementSchema]
            private static readonly Schema TheSchema = Schema.Of(
                    Mandatory.Of<Root>());

            [field: ForChild]
            private Root? Root { get; }
        }
    }
}

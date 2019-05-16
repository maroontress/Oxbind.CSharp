namespace Maroontress.Oxbind.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class InvalidDocumentTest
    {
        [TestMethod]
        public void ElementNotFound()
        {
            const string xml = ""
                + "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n"
                + "<root>\r\n"
                + "</root>\r\n";
            const string m = ""
                + "3:3: unexpected node type: EndElement of the element "
                + "'root' (it is expected that the element 'first' "
                + "starts)";
            Checks.ThrowBindException<Root>(xml, m);
        }

        [TestMethod]
        public void OtherElementFound()
        {
            const string xml = ""
                + "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n"
                + "<root>\r\n"
                + "  <second/>\r\n"
                + "</root>\r\n";
            const string m = ""
                + "3:4: unexpected node type: Element of the element "
                + "'second' (it is expected that the element 'first' starts)";
            Checks.ThrowBindException<Root>(xml, m);
        }

        [TestMethod]
        public void UnexpectedElementFound()
        {
            const string xml = ""
                + "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n"
                + "<root>\r\n"
                + "  <first/>\r\n"
                + "  <second/>\r\n"
                + "</root>\r\n";
            const string m = ""
                + "4:4: unexpected node type: Element of the element "
                + "'second' (it is expected that the element 'root' ends)";
            Checks.ThrowBindException<Root>(xml, m);
        }

        [TestMethod]
        public void EmptyRoot()
        {
            const string xml = ""
                + "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n"
                + "<root/>\r\n";
            const string m = ""
                + "2:2: element is empty: Element of the element 'root' "
                + "(it is expected that the element 'root' contains the "
                + "child element 'first')";
            Checks.ThrowBindException<Root>(xml, m);
        }

        [ForElement("root")]
        public sealed class Root
        {
            [ElementSchema]
            private static readonly Schema TheSchema = Schema.Of(
                    Mandatory.Of<First>());

            [field: ForChild]
            private First First { get; }
        }

        [ForElement("first")]
        public sealed class First
        {
            [field: ForAttribute("value")]
            public BindEvent<string> Value { get; }
        }
    }
}

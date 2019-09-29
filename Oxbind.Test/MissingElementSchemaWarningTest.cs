namespace Maroontress.Oxbind.Test
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class MissingElementSchemaWarningTest
    {
        private const string Xml = ""
            + "<content project=\"Oxbind.CSharp\">\r\n"
            + "  <page id=\"index\" title=\"Oxbind.CSharp\" nav=\"Top\" />\r\n"
            + "  <page id=\"releasenotes\" "
                + "title=\"Release Notes | Oxbind.CSharp\" "
                + "nav=\"Release Notes\" />\r\n"
            + "</content>\r\n";

        [TestMethod]
        public void CaseIgnoreWarnings()
        {
            var f = new OxbinderFactory(true);
            var b = f.Of<Content>();
            var m = "2:4: unexpected node type: Element of the element 'page' "
                + "(it is expected that the element 'content' ends)";
            Checks.ThrowBindException(b, Xml, m);
        }

        [TestMethod]
        public void CaseTreatWarningsAsErrors()
        {
            var m = "Content has failed to validate annotations: "
                + "Content: Warning: The field annotated with [ForChild] or "
                + "the method annotated with [FromChild] is found without "
                + "the static field annotated with [ElementSchema] (probably "
                + "missing [ElementSchema] annotation): "
                + "<Pages>k__BackingField";
            Checks.ThrowBindException<Content>(Xml, m);
        }

        [ForElement("content")]
        private sealed class Content
        {
            // [ElementSchema]
            public static readonly Schema TheSchema = Schema.Of(
                Multiple.Of<Page>());

            [field: ForAttribute("project")]
            public string? Project { get; }

            [field: ForChild]
            public IEnumerable<Page>? Pages { get; }
        }

        [ForElement("page")]
        private sealed class Page
        {
            [field: ForAttribute("id")]
            public string? Id { get; }

            [field: ForAttribute("title")]
            public string? Title { get; }

            [field: ForAttribute("nav")]
            public string? Nav { get; }
        }
    }
}

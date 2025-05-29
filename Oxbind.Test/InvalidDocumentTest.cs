namespace Maroontress.Oxbind.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class InvalidDocumentTest
{
    [TestMethod]
    public void ElementNotFound()
    {
        const string xml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <root>
            </root>
            """;
        const string m = """
            3:3: Unexpected node type: EndElement of the element 'root'. (Expected the start of element 'first'.)
            """;
        NewInstanceChecks.ThrowBindException<Root>(xml, m);
    }

    [TestMethod]
    public void OtherElementFound()
    {
        const string xml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <root>
              <second/>
            </root>
            """;
        const string m = """
            3:4: Unexpected node type: Element of the element 'second'. (Expected the start of element 'first'.)
            """;
        NewInstanceChecks.ThrowBindException<Root>(xml, m);
    }

    [TestMethod]
    public void UnexpectedElementFound()
    {
        const string xml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <root>
              <first/>
              <second/>
            </root>
            """;
        const string m = """
            4:4: Unexpected node type: Element of the element 'second'. (Expected the end of element 'root'.)
            """;
        NewInstanceChecks.ThrowBindException<Root>(xml, m);
    }

    [TestMethod]
    public void EmptyRoot()
    {
        const string xml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <root/>
            """;
        const string m = """
            2:2: Element 'root' is empty. (It was expected to contain the child element 'first'.)
            """;
        NewInstanceChecks.ThrowBindException<Root>(xml, m);
    }

    [ForElement("root")]
    public record class Root(
        [Required] First FirstChild);

    [ForElement("first")]
    public record class First(
        [ForAttribute("value")] BindResult<string>? AttributeValue);
}

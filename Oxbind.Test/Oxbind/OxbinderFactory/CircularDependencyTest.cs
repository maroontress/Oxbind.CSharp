namespace Maroontress.Oxbind.Test.Oxbind.OxbinderFactory;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class CircularDependencyTest
{
    [TestMethod]
    public void RootTest()
    {
        const string xml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <root>
              <first value="42"/>
              <second>text</second>
            </root>
            """;
        const string m = """
            Root has a circular dependency.
            """;
        Checks.ThrowBindException<Root>(xml, m);
    }

    [ForElement("root")]
    public record class Root(
        [Required] First FirstChild);

    [ForElement("first")]
    public record class First(
        [Required] Second SecondChild);

    [ForElement("second")]
    public record class Second(
        [Required] /*!?*/ Root RootChild);
}

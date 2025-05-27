namespace Maroontress.Oxbind.Test.Oxbind.OxbinderFactory;

using Maroontress.Oxbind;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class CircularDependencyTest
{
    [TestMethod]
    public void RootTest()
    {
        const string message = """
            Root has a circular dependency.
            """;
        var factory = new OxbinderFactory();
        Checks.ThrowBindException(
            () => _ = factory.Of<Root>(),
            "Of<Root>()",
            message);
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

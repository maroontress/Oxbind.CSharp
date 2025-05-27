namespace Maroontress.Oxbind.Test.Oxbind.OxbinderFactory;

using Maroontress.Oxbind;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class InvalidChildTest
{
    [TestMethod]
    public void RootTest()
    {
        const string message = """
            BadChild failed to validate annotations: BadChild: Error: A constructor parameter for a child element or text must be attributed with [Required], [Optional], [Multiple], or [ForText]: InnerText.
            """;
        var factory = new OxbinderFactory();
        Checks.ThrowBindException(
            () => _ = factory.Of<Root>(),
            "Of<Root>()",
            message);
        _ = factory.Of<GoodChild>();
        Checks.ThrowBindException(
            () => _ = factory.Of<BadChild>(),
            "Of<BadChild>()",
            message);
    }

    [ForElement("root")]
    public record class Root(
        [Required] GoodChild FirstChild,
        [Required] BadChild SecondChild);

    [ForElement("good")]
    public record class GoodChild(
        [ForText] string InnerText);

    [ForElement("bad")]
    public record class BadChild(
        /*!?*/ string InnerText);
}

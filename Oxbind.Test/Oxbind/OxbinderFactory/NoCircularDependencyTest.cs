namespace Maroontress.Oxbind.Test.Oxbind.OxbinderFactory;

using System.Collections.Generic;
using Maroontress.Oxbind;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class NoCircularDependencyTest
{
    [TestMethod]
    public void RootTest()
    {
        var factory = new OxbinderFactory();
        _ = factory.Of<Root>();
    }

    [ForElement("root")]
    public record class Root(
        [Required] Alpha FirstChild);

    [ForElement("alpha")]
    public record class Alpha(
        [Required] Beta FirstChild,
        [Optional] Root MaybeSecondChild);

    [ForElement("beta")]
    public record class Beta(
        [Multiple] IEnumerable<Root> FirstChildren);
}

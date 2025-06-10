namespace Maroontress.Oxbind.Test.Oxbind.Impl.Validator;

using Maroontress.Oxbind.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class InterfaceTest
{
    /* IRoot cannot be annotated with [ForElement] */
    public interface IRoot;

    [TestMethod]
    public void RootTest()
    {
        var logger = new Journal("IRoot");
        var v = Validators.New<IRoot>(logger);
        Assert.IsFalse(v.IsValid);
        Assert.Contains(
            """
            IRoot: Error: The type attributed with [ForElement] must be a class, not an interface.
            """,
            logger.GetMessages());
    }

    [ForElement("root")]
    public record class Root : IRoot;
}

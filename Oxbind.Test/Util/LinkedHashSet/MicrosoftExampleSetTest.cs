namespace Maroontress.Util.Test.LinkedHashSet
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class MicrosoftExampleSetTest
        : AbstractMicrosoftExampleSetTest
    {
        protected override ISet<int> NewSet()
        {
            return new LinkedHashSet<int>();
        }
    }
}

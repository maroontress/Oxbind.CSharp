namespace Maroontress.Util.Test.LinkedHashSet
{
    using System.Collections.Generic;
    using Maroontress.Util;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class SetConformanceTest : AbstractSetConformanceTest
    {
        protected override ISet<string> NewSet()
        {
            return new LinkedHashSet<string>(16);
        }
    }
}

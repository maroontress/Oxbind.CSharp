namespace Maroontress.Oxbind.Test.Util.LinkedHashSet;

using System.Collections.Generic;
using Maroontress.Oxbind.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class SetConformanceTest : AbstractSetConformanceTest
{
    protected override ISet<string> NewSet()
    {
        return new LinkedHashSet<string>(16);
    }
}

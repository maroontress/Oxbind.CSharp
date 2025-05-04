namespace Maroontress.Oxbind.Test.Util.LinkedHashSet;

using System.Collections.Generic;
using Maroontress.Oxbind.Util;
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

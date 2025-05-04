namespace Maroontress.Oxbind.Test.Util;

using System;
using Maroontress.Oxbind.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class InternMapTest
{
    [TestMethod]
    public void Intern_Func_1()
    {
        var map = new InternMap<int, string>();
        var key = 12;
        var v1 = "12";
        var v2 = string.Join("", new[] { "1", "2" });
        Assert.AreNotSame(v1, v2);
        var c1 = map.Intern(key, () => v1);
        Assert.AreSame(v1, c1);
        var c2 = map.Intern(key, () => v2);
        Assert.AreSame(c1, c2);
    }

    [TestMethod]
    public void Intern_Func_2()
    {
        var map = new InternMap<int, string>();
        var key = 12;
        var c1 = map.Intern(key, k => k.ToString());
        var c2 = map.Intern(key, k => k.ToString());
        Assert.AreSame(c1, c2);
    }

    [TestMethod]
    public void Intern_Func_1_Null()
    {
        var map = new InternMap<int, string>();
        Assert.ThrowsException<ArgumentNullException>(
            () => _ = map.Intern(12, (Func<string>)null!));
    }

    [TestMethod]
    public void Intern_Func_2_Null()
    {
        var map = new InternMap<int, string>();
        Assert.ThrowsException<ArgumentNullException>(
            () => _ = map.Intern(12, (Func<int, string>)null!));
    }
}

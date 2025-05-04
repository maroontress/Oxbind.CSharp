namespace Maroontress.Oxbind.Test.Util.LinkedHashSet;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

public abstract class AbstractSetConformanceTest
{
    public static bool SortedSequenceEqual<T>(
        IEnumerable<T> left, IEnumerable<T> right)
    {
        var sortedLeft = left.OrderBy(i => i);
        var sortedRight = right.OrderBy(i => i);
        return sortedLeft.SequenceEqual(sortedRight);
    }

    [TestMethod]
    public void InitialState()
    {
        var s = NewSet();
        Assert.AreEqual(0, s.Count);
        Assert.AreEqual(false, s.IsReadOnly);
        Assert.IsFalse(s.Any());
        Assert.IsFalse(s.Contains("foo"));
        var o = Array.Empty<string>();
        s.CopyTo(o, 0);
        Assert.IsFalse(s.Remove("foo"));
    }

    [TestMethod]
    public void CopyTo()
    {
        var s = NewSet();
        {
            var empty = Array.Empty<string>();
            Assert.ThrowsException<ArgumentException>(
                () => s.CopyTo(empty, 1));
        }
        s.Add("foo");
        {
            var empty = Array.Empty<string>();
            Assert.ThrowsException<ArgumentException>(
                () => s.CopyTo(empty, 0));
        }
        {
            var one = new string[1];
            Assert.ThrowsException<ArgumentException>(
                () => s.CopyTo(one, 1));
        }
        s.Add("bar");
        {
            var one = new string[1];
            Assert.ThrowsException<ArgumentException>(
                () => s.CopyTo(one, 0));
        }
        {
            var two = new string[2];
            Assert.ThrowsException<ArgumentException>(
                () => s.CopyTo(two, 1));
        }
    }

    [TestMethod]
    public void CopyTo_ArgumentOutOfRangeException()
    {
        var s = NewSet();
        {
            var empty = Array.Empty<string>();
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => s.CopyTo(empty, -1));
        }
    }

    [TestMethod]
    public void CopyTo_Null()
    {
        var s = NewSet();
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => s.CopyTo(null!, 0));
        }
    }

    [TestMethod]
    public void AddOne()
    {
        var s = NewSet();
        Assert.IsTrue(s.Add("foo"));
        Assert.AreEqual(1, s.Count);
        Assert.IsTrue(s.Any());
        Assert.IsTrue(s.Contains("foo"));
        Assert.IsFalse(s.Contains("bar"));
        var o = new string[1];
        s.CopyTo(o, 0);
        Assert.AreEqual("foo", o[0]);
        Assert.IsFalse(s.Remove("bar"));
        Assert.IsTrue(s.Remove("foo"));
        Assert.IsFalse(s.Remove("foo"));
        Assert.AreEqual(0, s.Count);
    }

    [TestMethod]
    public void AddTwo()
    {
        var s = NewSet();
        Assert.IsTrue(s.Add("foo"));
        Assert.IsFalse(s.Add("foo"));
        Assert.AreEqual(1, s.Count);
        Assert.IsTrue(s.Add("bar"));
        Assert.IsFalse(s.Add("bar"));
        Assert.AreEqual(2, s.Count);
        Assert.IsTrue(s.Contains("foo"));
        Assert.IsTrue(s.Contains("bar"));
        Assert.IsFalse(s.Contains("baz"));
        var o = new string[2];
        s.CopyTo(o, 0);
        Array.Sort(o);
        Assert.IsTrue(SortedSequenceEqual(Create("bar", "foo"), o));
        Assert.IsFalse(s.Remove("baz"));
        Assert.IsTrue(s.Remove("foo"));
        Assert.IsFalse(s.Remove("foo"));
        Assert.AreEqual(1, s.Count);
        Assert.IsTrue(s.Remove("bar"));
        Assert.IsFalse(s.Remove("bar"));
        Assert.AreEqual(0, s.Count);
    }

    [TestMethod]
    public void Clear()
    {
        var s = NewSet();
        Assert.IsTrue(s.Add("foo"));
        Assert.IsTrue(s.Add("bar"));
        Assert.IsTrue(s.Add("baz"));
        Assert.AreEqual(3, s.Count);
        Assert.IsTrue(s.Any());
        s.Clear();
        Assert.AreEqual(0, s.Count);
        Assert.IsFalse(s.Any());
    }

    [TestMethod]
    public void ExceptWith()
    {
        var s = NewSet();
        s.Add("foo");
        s.Add("bar");
        s.ExceptWith(Create("bar", "baz"));
        Assert.IsTrue(SortedSequenceEqual(Create("foo"), s));
    }

    [TestMethod]
    public void ExceptWith_Null()
    {
        var all = Create("foo", "bar");
        var s = NewSet();
        s.UnionWith(all);
        Assert.ThrowsException<ArgumentNullException>(
            () => s.ExceptWith(null!));
    }

    [TestMethod]
    public void IntersectWith()
    {
        var all = Create("foo", "bar", "baz");
        var s = NewSet();
        s.UnionWith(all);
        s.IntersectWith(Create("bar"));
        Assert.IsTrue(SortedSequenceEqual(Create("bar"), s));
    }

    [TestMethod]
    public void IntersectWith_DoubleAdd()
    {
        var all = Create("foo", "bar", "baz");
        var s = NewSet();
        s.UnionWith(all);
        s.IntersectWith(Create("bar", "bar"));
        Assert.IsTrue(SortedSequenceEqual(Create("bar"), s));
    }

    [TestMethod]
    public void IntersectWith_4x2()
    {
        var all = Create("foo", "bar", "baz", "barBaz");
        var s = NewSet();
        s.UnionWith(all);
        s.IntersectWith(Create("bar", "baz"));
        Assert.IsTrue(SortedSequenceEqual(Create("bar", "baz"), s));
    }

    [TestMethod]
    public void IntersectWith_RemoveTail()
    {
        var all = Create("foo", "bar");
        var s = NewSet();
        s.UnionWith(all);
        s.IntersectWith(Create("foo", "baz"));
        Assert.IsTrue(SortedSequenceEqual(Create("foo"), s));
    }

    [TestMethod]
    public void IntersectWith_RemoveHead()
    {
        var all = Create("foo", "bar");
        var s = NewSet();
        s.UnionWith(all);
        s.IntersectWith(Create("bar", "baz"));
        Assert.IsTrue(SortedSequenceEqual(Create("bar"), s));
    }

    [TestMethod]
    public void IntersectWith_Self()
    {
        var all = Create("foo", "bar", "baz");
        var s = NewSet();
        s.UnionWith(all);
        s.IntersectWith(s);
        Assert.IsTrue(SortedSequenceEqual(all, s));
    }

    [TestMethod]
    public void IntersectWith_Null()
    {
        var all = Create("foo", "bar");
        var s = NewSet();
        s.UnionWith(all);
        Assert.ThrowsException<ArgumentNullException>(
            () => s.IntersectWith(null!));
    }

    [TestMethod]
    public void IsProperSubsetOf()
    {
        static Func<IEnumerable<T>, bool> M<T>(ISet<T> set)
            => a => set.IsProperSubsetOf(a);

        var s = NewSet();
        var m = M(s);
        Assert.IsFalse(m(ImmutableArray<string>.Empty));
        Assert.IsTrue(m(Create("foo")));
        s.Add("foo");
        s.Add("bar");
        Assert.IsFalse(m(ImmutableArray<string>.Empty));
        Assert.IsFalse(m(Create("foo")));
        Assert.IsFalse(m(Create("baz")));
        Assert.IsFalse(m(Create("foo", "bar")));
        Assert.IsFalse(m(Create("foo", "baz")));
        Assert.IsTrue(m(Create("foo", "bar", "baz")));
        Assert.IsTrue(m(Create("foo", "bar", "foo", "baz")));
    }

    [TestMethod]
    public void IsProperSubsetOf_Null()
    {
        var all = Create("foo", "bar");
        var s = NewSet();
        s.UnionWith(all);
        Assert.ThrowsException<ArgumentNullException>(
            () => s.IsProperSubsetOf(null!));
    }

    [TestMethod]
    public void IsProperSubsetOf_Self()
    {
        var all = Create("foo", "bar");
        var s = NewSet();
        s.UnionWith(all);
        Assert.IsFalse(s.IsProperSubsetOf(s));
    }

    [TestMethod]
    public void IsProperSupersetOf()
    {
        static Func<IEnumerable<T>, bool> M<T>(ISet<T> set)
            => a => set.IsProperSupersetOf(a);

        var s = NewSet();
        var m = M(s);
        Assert.IsFalse(m(ImmutableArray<string>.Empty));
        Assert.IsFalse(m(Create("foo")));
        s.Add("foo");
        s.Add("bar");
        Assert.IsTrue(m(ImmutableArray<string>.Empty));
        Assert.IsTrue(m(Create("foo")));
        Assert.IsFalse(m(Create("baz")));
        Assert.IsFalse(m(Create("foo", "bar")));
        Assert.IsFalse(m(Create("foo", "baz")));
        Assert.IsFalse(m(Create("foo", "bar", "baz")));
    }

    [TestMethod]
    public void IsProperSupersetOf_Null()
    {
        var all = Create("foo", "bar");
        var s = NewSet();
        s.UnionWith(all);
        Assert.ThrowsException<ArgumentNullException>(
            () => s.IsProperSupersetOf(null!));
    }

    [TestMethod]
    public void IsProperSupersetOf_Self()
    {
        var all = Create("foo", "bar");
        var s = NewSet();
        s.UnionWith(all);
        Assert.IsFalse(s.IsProperSupersetOf(s));
    }

    [TestMethod]
    public void IsSubsetOf()
    {
        static Func<IEnumerable<T>, bool> M<T>(ISet<T> set)
            => a => set.IsSubsetOf(a);

        var s = NewSet();
        var m = M(s);
        Assert.IsTrue(m(ImmutableArray<string>.Empty));
        Assert.IsTrue(m(Create("foo")));
        s.Add("foo");
        s.Add("bar");
        Assert.IsFalse(m(ImmutableArray<string>.Empty));
        Assert.IsFalse(m(Create("foo")));
        Assert.IsFalse(m(Create("baz")));
        Assert.IsTrue(m(Create("foo", "bar")));
        Assert.IsFalse(m(Create("foo", "baz")));
        Assert.IsTrue(m(Create("foo", "bar", "baz")));
        Assert.IsTrue(m(Create("foo", "foo", "bar", "baz")));
    }

    [TestMethod]
    public void IsSubsetOf_Null()
    {
        var all = Create("foo", "bar");
        var s = NewSet();
        s.UnionWith(all);
        Assert.ThrowsException<ArgumentNullException>(
            () => s.IsSubsetOf(null!));
    }

    [TestMethod]
    public void IsSubsetOf_Self()
    {
        var all = Create("foo", "bar");
        var s = NewSet();
        s.UnionWith(all);
        Assert.IsTrue(s.IsSubsetOf(s));
    }

    [TestMethod]
    public void IsSupersetOf()
    {
        static Func<IEnumerable<T>, bool> M<T>(ISet<T> set)
            => a => set.IsSupersetOf(a);

        var s = NewSet();
        var m = M(s);
        Assert.IsTrue(m(ImmutableArray<string>.Empty));
        Assert.IsFalse(m(Create("foo")));
        s.Add("foo");
        s.Add("bar");
        Assert.IsTrue(m(ImmutableArray<string>.Empty));
        Assert.IsTrue(m(Create("foo")));
        Assert.IsTrue(m(Create("foo", "bar")));
        Assert.IsFalse(m(Create("foo", "bar", "baz")));
    }

    [TestMethod]
    public void IsSupersetOf_Null()
    {
        var all = Create("foo", "bar");
        var s = NewSet();
        s.UnionWith(all);
        Assert.ThrowsException<ArgumentNullException>(
            () => s.IsSupersetOf(null!));
    }

    [TestMethod]
    public void IsSupersetOf_Self()
    {
        var all = Create("foo", "bar");
        var s = NewSet();
        s.UnionWith(all);
        Assert.IsTrue(s.IsSupersetOf(s));
    }

    [TestMethod]
    public void Overlaps()
    {
        static Func<IEnumerable<T>, bool> M<T>(ISet<T> set)
            => a => set.Overlaps(a);

        var s = NewSet();
        var m = M(s);
        Assert.IsFalse(m(ImmutableArray<string>.Empty));
        Assert.IsFalse(m(Create("foo")));
        s.Add("foo");
        s.Add("bar");
        Assert.IsFalse(m(ImmutableArray<string>.Empty));
        Assert.IsTrue(m(Create("foo")));
        Assert.IsTrue(m(Create("foo", "bar")));
        Assert.IsTrue(m(Create("foo", "bar", "baz")));
    }

    [TestMethod]
    public void Overlaps_Empty()
    {
        var s = NewSet();
        s.Overlaps(Create("foo", "bar"));
        Assert.AreEqual(0, s.Count);
    }

    [TestMethod]
    public void Overlaps_Self()
    {
        var all = Create("foo", "bar");
        var s = NewSet();
        s.UnionWith(all);
        Assert.IsTrue(s.Overlaps(s));
    }

    [TestMethod]
    public void Overlaps_Null()
    {
        var all = Create("foo", "bar");
        var s = NewSet();
        s.UnionWith(all);
        Assert.ThrowsException<ArgumentNullException>(
            () => s.Overlaps(null!));
    }

    [TestMethod]
    public void SetEquals()
    {
        static Func<IEnumerable<T>, bool> M<T>(ISet<T> set)
            => a => set.SetEquals(a);

        var s = NewSet();
        var m = M(s);
        Assert.IsTrue(m(ImmutableArray<string>.Empty));
        Assert.IsFalse(m(Create("foo")));
        s.Add("foo");
        s.Add("bar");
        Assert.IsFalse(m(ImmutableArray<string>.Empty));
        Assert.IsFalse(m(Create("foo")));
        Assert.IsFalse(m(Create("baz")));
        Assert.IsTrue(m(Create("foo", "bar")));
        Assert.IsTrue(m(Create("foo", "bar", "foo")));
        Assert.IsFalse(m(Create("foo", "baz")));
        Assert.IsFalse(m(Create("foo", "bar", "baz")));
    }

    [TestMethod]
    public void SetEquals_Null()
    {
        var all = Create("foo", "bar");
        var s = NewSet();
        s.UnionWith(all);
        Assert.ThrowsException<ArgumentNullException>(
            () => s.SetEquals(null!));
    }

    [TestMethod]
    public void SetEquals_Self()
    {
        var all = Create("foo", "bar");
        var s = NewSet();
        s.UnionWith(all);
        Assert.IsTrue(s.SetEquals(s));
    }

    [TestMethod]
    public void SymmetricExceptWith()
    {
        var all = Create("foo", "bar");
        var s = NewSet();
        s.UnionWith(all);
        s.SymmetricExceptWith(Create("bar", "baz"));
        Assert.IsTrue(SortedSequenceEqual(Create("baz", "foo"), s));
    }

    [TestMethod]
    public void SymmetricExceptWith_AddOnly_2x1()
    {
        var all = Create("foo", "bar");
        var s = NewSet();
        s.UnionWith(all);
        s.SymmetricExceptWith(Create("baz"));
        Assert.IsTrue(SortedSequenceEqual(Create("bar", "baz", "foo"), s));
    }

    [TestMethod]
    public void SymmetricExceptWith_AddOnly_1x2()
    {
        var all = Create("foo");
        var s = NewSet();
        s.UnionWith(all);
        s.SymmetricExceptWith(Create("bar", "baz"));
        Assert.IsTrue(SortedSequenceEqual(Create("bar", "baz", "foo"), s));
    }

    [TestMethod]
    public void SymmetricExceptWith_RemoveOnly()
    {
        var all = Create("foo", "bar", "baz");
        var s = NewSet();
        s.UnionWith(all);
        s.SymmetricExceptWith(Create("bar"));
        Assert.IsTrue(SortedSequenceEqual(Create("baz", "foo"), s));
    }

    [TestMethod]
    public void SymmetricExceptWith_RemoveOnly_Tail()
    {
        var all = Create("foo", "bar", "baz");
        var s = NewSet();
        s.UnionWith(all);
        s.SymmetricExceptWith(Create("bar", "baz"));
        Assert.IsTrue(SortedSequenceEqual(Create("foo"), s));
    }

    [TestMethod]
    public void SymmetricExceptWith_RemoveOnly_Head()
    {
        var all = Create("foo", "bar", "baz");
        var s = NewSet();
        s.UnionWith(all);
        s.SymmetricExceptWith(Create("foo", "bar"));
        Assert.IsTrue(SortedSequenceEqual(Create("baz"), s));
    }

    [TestMethod]
    public void SymmetricExceptWith_DoubleAdd()
    {
        var all = Create("foo", "bar");
        var s = NewSet();
        s.UnionWith(all);
        s.SymmetricExceptWith(Create("bar", "baz", "baz"));
        Assert.IsTrue(SortedSequenceEqual(Create("baz", "foo"), s));
    }

    [TestMethod]
    public void SymmetricExceptWith_DoubleRemove()
    {
        var all = Create("foo", "bar");
        var s = NewSet();
        s.UnionWith(all);
        s.SymmetricExceptWith(Create("bar", "bar", "baz"));
        Assert.IsTrue(SortedSequenceEqual(Create("baz", "foo"), s));
    }

    [TestMethod]
    public void SymmetricExceptWith_Equal()
    {
        var all = Create("foo", "bar");
        var s = NewSet();
        s.UnionWith(all);
        s.SymmetricExceptWith(all);
        Assert.AreEqual(0, s.Count);
    }

    [TestMethod]
    public void SymmetricExceptWith_Self()
    {
        var all = Create("foo", "bar");
        var s = NewSet();
        s.UnionWith(all);
        s.SymmetricExceptWith(s);
        Assert.AreEqual(0, s.Count);
    }

    [TestMethod]
    public void SymmetricExceptWith_Null()
    {
        var all = Create("foo", "bar");
        var s = NewSet();
        s.UnionWith(all);
        Assert.ThrowsException<ArgumentNullException>(
            () => s.SymmetricExceptWith(null!));
    }

    [TestMethod]
    public void UnionWith()
    {
        var all = Create("foo", "bar");
        var s = NewSet();
        s.UnionWith(all);
        s.UnionWith(Create("bar", "baz"));
        Assert.IsTrue(SortedSequenceEqual(Create("bar", "baz", "foo"), s));
    }

    [TestMethod]
    public void UnionWith_Self()
    {
        var all = Create("foo", "bar");
        var s = NewSet();
        s.UnionWith(all);
        s.UnionWith(s);
        Assert.IsTrue(SortedSequenceEqual(all, s));
    }

    [TestMethod]
    public void UnionWith_Null()
    {
        var all = Create("foo", "bar");
        var s = NewSet();
        s.UnionWith(all);
        Assert.ThrowsException<ArgumentNullException>(
            () => s.UnionWith(null!));
    }

    [TestMethod]
    public void ICollection_T_Add()
    {
        var s = NewSet();
        var c = (ICollection<string>)s;
        c.Add("foo");
        c.Add("bar");
        c.Add("foo");
        Assert.IsTrue(SortedSequenceEqual(Create("bar", "foo"), s));
    }

    [TestMethod]
    public void IEnumerable_GetEnumerator()
    {
        var all = Create("foo", "bar");
        var s = NewSet();
        s.UnionWith(all);
        var e = (IEnumerable)s;

        var list = new List<string>();
        foreach (string i in e)
        {
            list.Add(i);
        }
        Assert.IsTrue(SortedSequenceEqual(list, s));
    }

    protected abstract ISet<string> NewSet();

    private static ImmutableArray<T> Create<T>(params T[] all)
    {
        return ImmutableArray.Create(all);
    }
}

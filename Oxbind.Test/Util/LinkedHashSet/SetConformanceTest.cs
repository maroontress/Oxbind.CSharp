namespace Maroontress.Util.LinkedHashSet.Test
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Maroontress.Util;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class SetConformanceTest
    {
        private static IEnumerable<string> Empty { get; }
            = Enumerable.Empty<string>();

        [TestMethod]
        public void IsCompliantToMutableSet()
        {
            IsCompliantToMutableSet(() => new LinkedHashSet<string>());
        }

        private static void IsCompliantToMutableSet(Func<ISet<string>> newSet)
        {
            // void ExceptWith(IEnumerable<T> other);
            {
                var s = newSet();
                s.Add("foo");
                s.Add("bar");
                s.ExceptWith(ImmutableArray.Create("bar", "baz"));
                Assert.AreEqual(1, s.Count);
                var o = new string[1];
                s.CopyTo(o, 0);
                Array.Sort(o);
                Assert.IsTrue(Enumerable.SequenceEqual(
                    new[] { "foo", }, o));
            }

            // void IntersectWith(IEnumerable<T> other);
            {
                var s = newSet();
                s.Add("foo");
                s.Add("bar");
                s.IntersectWith(ImmutableArray.Create("bar", "bar", "baz"));
                Assert.AreEqual(1, s.Count);
                var o = new string[1];
                s.CopyTo(o, 0);
                Array.Sort(o);
                Assert.IsTrue(Enumerable.SequenceEqual(
                    new[] { "bar", }, o));
            }

            // bool IsProperSubsetOf(IEnumerable<T> other);
            {
                static Func<IEnumerable<T>, bool> M<T>(ISet<T> set)
                    => a => set.IsProperSubsetOf(a);
                var s = newSet();
                var m = M(s);
                Assert.IsFalse(m(Empty));
                Assert.IsTrue(m(ImmutableArray.Create("foo")));
                s.Add("foo");
                s.Add("bar");
                Assert.IsFalse(m(Empty));
                Assert.IsFalse(m(ImmutableArray.Create("foo")));
                Assert.IsFalse(m(ImmutableArray.Create("foo", "bar")));
                Assert.IsTrue(m(ImmutableArray.Create("foo", "bar", "baz")));
            }

            // bool IsProperSupersetOf(IEnumerable<T> other);
            {
                static Func<IEnumerable<T>, bool> M<T>(ISet<T> set)
                    => a => set.IsProperSupersetOf(a);
                var s = newSet();
                var m = M(s);
                Assert.IsFalse(m(Empty));
                Assert.IsFalse(m(ImmutableArray.Create("foo")));
                s.Add("foo");
                s.Add("bar");
                Assert.IsTrue(m(Empty));
                Assert.IsTrue(m(ImmutableArray.Create("foo")));
                Assert.IsFalse(m(ImmutableArray.Create("foo", "bar")));
                Assert.IsFalse(m(ImmutableArray.Create("foo", "bar", "baz")));
            }

            // bool IsSubsetOf(IEnumerable<T> other);
            {
                static Func<IEnumerable<T>, bool> M<T>(ISet<T> set)
                    => a => set.IsSubsetOf(a);
                var s = newSet();
                var m = M(s);
                Assert.IsTrue(m(Empty));
                Assert.IsTrue(m(ImmutableArray.Create("foo")));
                s.Add("foo");
                s.Add("bar");
                Assert.IsFalse(m(Empty));
                Assert.IsFalse(m(ImmutableArray.Create("foo")));
                Assert.IsTrue(m(ImmutableArray.Create("foo", "bar")));
                Assert.IsTrue(m(ImmutableArray.Create("foo", "bar", "baz")));
            }

            // bool IsSupersetOf(IEnumerable<T> other);
            {
                static Func<IEnumerable<T>, bool> M<T>(ISet<T> set)
                    => a => set.IsSupersetOf(a);
                var s = newSet();
                var m = M(s);
                Assert.IsTrue(m(Empty));
                Assert.IsFalse(m(ImmutableArray.Create("foo")));
                s.Add("foo");
                s.Add("bar");
                Assert.IsTrue(m(Empty));
                Assert.IsTrue(m(ImmutableArray.Create("foo")));
                Assert.IsTrue(m(ImmutableArray.Create("foo", "bar")));
                Assert.IsFalse(m(ImmutableArray.Create("foo", "bar", "baz")));
            }

            // bool Overlaps(IEnumerable<T> other);
            {
                static Func<IEnumerable<T>, bool> M<T>(ISet<T> set)
                    => a => set.Overlaps(a);
                var s = newSet();
                var m = M(s);
                Assert.IsFalse(m(Empty));
                Assert.IsFalse(m(ImmutableArray.Create("foo")));
                s.Add("foo");
                s.Add("bar");
                Assert.IsFalse(m(Empty));
                Assert.IsTrue(m(ImmutableArray.Create("foo")));
                Assert.IsTrue(m(ImmutableArray.Create("foo", "bar")));
                Assert.IsTrue(m(ImmutableArray.Create("foo", "bar", "baz")));
            }

            // bool SetEquals(IEnumerable<T> other);
            {
                static Func<IEnumerable<T>, bool> M<T>(ISet<T> set)
                    => a => set.SetEquals(a);
                var s = newSet();
                var m = M(s);
                Assert.IsTrue(m(Empty));
                Assert.IsFalse(m(ImmutableArray.Create("foo")));
                s.Add("foo");
                s.Add("bar");
                Assert.IsFalse(m(Empty));
                Assert.IsFalse(m(ImmutableArray.Create("foo")));
                Assert.IsTrue(m(ImmutableArray.Create("foo", "bar")));
                Assert.IsFalse(m(ImmutableArray.Create("foo", "bar", "baz")));
            }

            // void SymmetricExceptWith(IEnumerable<T> other);
            {
                var s = newSet();
                s.Add("foo");
                s.Add("bar");
                s.SymmetricExceptWith(ImmutableArray.Create("bar", "baz"));
                Assert.AreEqual(2, s.Count);
                var o = new string[2];
                s.CopyTo(o, 0);
                Array.Sort(o);
                Assert.IsTrue(Enumerable.SequenceEqual(
                    new[] { "baz", "foo", }, o));
            }

            // void UnionWith(IEnumerable<T> other);
            {
                var s = newSet();
                s.Add("foo");
                s.Add("bar");
                s.UnionWith(ImmutableArray.Create("bar", "baz"));
                Assert.AreEqual(3, s.Count);
                var o = new string[3];
                s.CopyTo(o, 0);
                Array.Sort(o);
                Assert.IsTrue(Enumerable.SequenceEqual(
                    new[] { "bar", "baz", "foo", }, o));
            }
        }
    }
}

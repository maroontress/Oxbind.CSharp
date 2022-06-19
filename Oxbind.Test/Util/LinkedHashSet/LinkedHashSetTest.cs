namespace Maroontress.Util.Test.LinkedHashSet
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class LinkedHashSetTest
    {
        [TestMethod]
        public void EnsureCapacity()
        {
            var n = 1000;
            var all = Enumerable.Range(0, n);
            var s = NewLinkedHashSet<int>();
            s.UnionWith(all);
            Assert.AreEqual(s.Count, n);
            Assert.IsTrue(Enumerable.SequenceEqual(all, s));
        }

        [TestMethod]
        public void InvalidInitialCapacity()
        {
            Assert.ThrowsException<ArgumentException>(
                () => _ = NewLinkedHashSet<string>(-1));
        }

        [TestMethod]
        public void AddAndRemove()
        {
            var s = NewLinkedHashSet<string>();
            s.UnionWith(Create("foo", "bar", "baz"));
            Assert.IsTrue(s.SequenceEqual(Create("foo", "bar", "baz")));
            Assert.IsFalse(s.Add("bar"));
            Assert.IsTrue(s.SequenceEqual(Create("foo", "bar", "baz")));
            Assert.IsTrue(s.Remove("bar"));
            Assert.IsTrue(s.Add("bar"));
            Assert.IsTrue(s.SequenceEqual(Create("foo", "baz", "bar")));
        }

        [TestMethod]
        public void IntersectWith()
        {
            var s = NewLinkedHashSet<string>();
            s.UnionWith(Create("foo", "bar", "baz"));
            s.IntersectWith(Create("baz", "bar", "fooBar"));
            Assert.IsTrue(s.SequenceEqual(Create("bar", "baz")));
        }

        [TestMethod]
        public void IntersectWith_LeftEmpty()
        {
            var s = NewLinkedHashSet<string>();
            s.IntersectWith(Create("baz", "bar", "fooBar"));
            Assert.AreEqual(0, s.Count);
        }

        [TestMethod]
        public void IntersectWith_Self()
        {
            var all = Create("foo", "bar", "baz");
            var s = NewLinkedHashSet<string>();
            s.UnionWith(all);
            s.IntersectWith(s);
            Assert.IsTrue(s.SequenceEqual(all));
        }

        [TestMethod]
        public void IntersectWith_Null()
        {
            var n = (LinkedHashSet<string>?)null;
            var s = NewLinkedHashSet<string>();
            Assert.ThrowsException<ArgumentNullException>(
                () => s.IntersectWith(n!));
        }

        [TestMethod]
        public void IntersectWith_LinkedHashSet()
        {
            var left = Create("foo", "bar", "baz");
            var right = Create("fooBar", "baz", "bar");
            var center = Create("bar", "baz");
            var s1 = NewLinkedHashSet<string>();
            var s2 = NewLinkedHashSet<string>();
            s1.UnionWith(left);
            s2.UnionWith(right);
            s1.IntersectWith(s2);
            Assert.IsTrue(s1.SequenceEqual(center));
            Assert.IsTrue(s2.SequenceEqual(right));
        }

        [TestMethod]
        public void IntersectWith_LinkedHashSet_LeftEmpty()
        {
            var right = Create("fooBar", "baz", "bar");
            var s1 = NewLinkedHashSet<string>();
            var s2 = NewLinkedHashSet<string>();
            s2.UnionWith(right);
            s1.IntersectWith(s2);
            Assert.AreEqual(0, s1.Count);
        }

        [TestMethod]
        public void IntersectWith_LinkedHashSet_RightEmpty()
        {
            var left = Create("foo", "bar", "baz");
            var s1 = NewLinkedHashSet<string>();
            var s2 = NewLinkedHashSet<string>();
            s1.UnionWith(left);
            s1.IntersectWith(s2);
            Assert.AreEqual(0, s1.Count);
        }

        [TestMethod]
        public void ExceptWith()
        {
            var s = NewLinkedHashSet<string>();
            s.UnionWith(Create("foo", "bar", "baz"));
            s.ExceptWith(Create("barBaz", "bar", "fooBar"));
            Assert.IsTrue(s.SequenceEqual(Create("foo", "baz")));
        }

        [TestMethod]
        public void SymmetricExceptWith()
        {
            var s = NewLinkedHashSet<string>();
            s.UnionWith(Create("foo", "bar", "baz"));
            s.SymmetricExceptWith(
                Create("barBaz", "bar", "fooBar", "bar", "barBaz"));
            Assert.IsTrue(s.SequenceEqual(
                Create("foo", "baz", "barBaz", "fooBar")));
        }

        [TestMethod]
        public void SymmetricExceptWith_EmptyLeft()
        {
            var right = Create("barBaz", "bar", "fooBar", "bar", "barBaz");
            var unique = Create("barBaz", "bar", "fooBar");
            var s = NewLinkedHashSet<string>();
            s.SymmetricExceptWith(right);
            Assert.IsTrue(s.SequenceEqual(unique));
        }

        [TestMethod]
        public void SymmetricExceptWith_Rehash()
        {
            var right = Enumerable.Range(5, 95)
                .Select(i => i.ToString());
            var s = NewLinkedHashSet<string>();
            s.UnionWith(Enumerable.Range(0, 10)
                .Select(i => i.ToString()));
            s.SymmetricExceptWith(right.Concat(right));
            Assert.IsTrue(s.SequenceEqual(
                Enumerable.Range(0, 5)
                    .Concat(Enumerable.Range(10, 90))
                    .Select(i => i.ToString())));
        }

        [TestMethod]
        public void SymmetricExceptWith_Self()
        {
            var s = NewLinkedHashSet<string>();
            s.UnionWith(Create("foo", "bar", "baz"));
            s.SymmetricExceptWith(s);
            Assert.AreEqual(0, s.Count);
        }

        [TestMethod]
        public void SymmetricExceptWith_Null()
        {
            var n = (LinkedHashSet<string>?)null;
            var s = NewLinkedHashSet<string>();
            Assert.ThrowsException<ArgumentNullException>(
                () => s.SymmetricExceptWith(n!));
        }

        [TestMethod]
        public void SymmetricExceptWith_LinkedHashSet()
        {
            var left = Create("foo", "bar", "baz");
            var right = Create("fooBar", "baz", "bar");
            var outside = Create("foo", "fooBar");
            var s1 = NewLinkedHashSet<string>();
            var s2 = NewLinkedHashSet<string>();
            s1.UnionWith(left);
            s2.UnionWith(right);
            s1.SymmetricExceptWith(s2);
            Assert.IsTrue(s1.SequenceEqual(outside));
            Assert.IsTrue(s2.SequenceEqual(right));
        }

        [TestMethod]
        public void SymmetricExceptWith_LinkedHashSet_Empty()
        {
            var right = Create("foo", "bar", "baz");
            var s1 = NewLinkedHashSet<string>();
            var s2 = NewLinkedHashSet<string>();
            s2.UnionWith(right);
            s1.SymmetricExceptWith(s2);
            Assert.IsTrue(s1.SequenceEqual(right));
            Assert.IsTrue(s2.SequenceEqual(right));
        }

        [TestMethod]
        public void UnionWith()
        {
            var s = NewLinkedHashSet<string>();
            s.UnionWith(Create("foo", "bar", "baz"));
            s.UnionWith(Create("barBaz", "bar", "fooBar"));
            Assert.IsTrue(s.SequenceEqual(
                Create("foo", "bar", "baz", "barBaz", "fooBar")));
        }

        [TestMethod]
        public void UnionWith_LinkedHashSet_Null()
        {
            var right = (LinkedHashSet<string>?)null;
            var s = NewLinkedHashSet<string>();
            s.UnionWith(Create("foo", "bar", "baz"));
            Assert.ThrowsException<ArgumentNullException>(
                () => s.UnionWith(right!));
        }

        [TestMethod]
        public void UnionWith_LinkedHashSet()
        {
            var s1 = NewLinkedHashSet<string>();
            var s2 = NewLinkedHashSet<string>();
            s1.UnionWith(Create("foo", "bar", "baz"));
            s2.UnionWith(Create("barBaz", "bar", "fooBar"));
            s1.UnionWith(s2);
            Assert.IsTrue(s1.SequenceEqual(
                Create("foo", "bar", "baz", "barBaz", "fooBar")));
        }

        [TestMethod]
        public void UnionWith_LinkedHashSet_BigRight()
        {
            var s1 = NewLinkedHashSet<string>();
            var s2 = NewLinkedHashSet<string>();
            s2.UnionWith(Enumerable.Range(0, 100)
                .Select(i => i.ToString()));
            s1.UnionWith(s2);
            Assert.IsTrue(s1.SequenceEqual(s2));
            Assert.AreEqual(s2.Count, s1.Count);
        }

        [TestMethod]
        public void Overlaps_LinkedHashSet()
        {
            static bool M<T>(LinkedHashSet<T> s1, LinkedHashSet<T> s2)
                    where T : notnull
                => s1.Overlaps(s2);

            var empty = NewLinkedHashSet<string>();
            var s1 = NewLinkedHashSet<string>();
            var s2 = NewLinkedHashSet<string>();
            var s3 = NewLinkedHashSet<string>();
            s1.UnionWith(Create("foo", "bar"));
            s2.UnionWith(Create("fooBar", "bar", "barBaz"));
            s3.UnionWith(Create("fooBar", "barBaz"));
            Assert.IsTrue(M(s1, s2));
            Assert.IsFalse(M(s1, s3));
            Assert.IsTrue(M(s1, s1));
            Assert.IsFalse(M(s1, empty));
            Assert.IsFalse(M(empty, s1));
        }

        [TestMethod]
        public void Overlaps_Null()
        {
            var n = (LinkedHashSet<string>?)null;
            var s = NewLinkedHashSet<string>();
            Assert.ThrowsException<ArgumentNullException>(
                () => _ = s.Overlaps(n!));
        }

        [TestMethod]
        public void IsSubsetOf_LinkedHashSet()
        {
            static bool M<T>(LinkedHashSet<T> s1, LinkedHashSet<T> s2)
                    where T : notnull
                => s1.IsSubsetOf(s2);

            var empty = NewLinkedHashSet<string>();
            var s1 = NewLinkedHashSet<string>();
            var s2 = NewLinkedHashSet<string>();
            var s3 = NewLinkedHashSet<string>();
            var s4 = NewLinkedHashSet<string>();
            var s5 = NewLinkedHashSet<string>();
            s1.UnionWith(Create("foo", "bar", "baz"));
            s2.UnionWith(Create("foo", "bar", "baz", "fooBar"));
            s3.UnionWith(Create("foo", "bar"));
            s4.UnionWith(Create("fooBar", "barBaz", "fooBar"));
            s5.UnionWith(Create("foo", "bar", "baz"));
            Assert.IsTrue(M(s1, s2));
            Assert.IsFalse(M(s1, s3));
            Assert.IsFalse(M(s1, s4));
            Assert.IsTrue(M(s1, s5));

            Assert.IsTrue(M(s1, s1));
            Assert.IsFalse(M(s1, empty));
            Assert.IsTrue(M(empty, s1));
            Assert.IsTrue(M(empty, NewLinkedHashSet<string>()));
        }

        [TestMethod]
        public void IsSubsetOf_Null()
        {
            var n = (LinkedHashSet<string>?)null;
            var s = NewLinkedHashSet<string>();
            Assert.ThrowsException<ArgumentNullException>(
                () => _ = s.IsSubsetOf(n!));
        }

        [TestMethod]
        public void IsProperSubsetOf_LinkedHashSet()
        {
            static bool M<T>(LinkedHashSet<T> s1, LinkedHashSet<T> s2)
                    where T : notnull
                => s1.IsProperSubsetOf(s2);

            var empty = NewLinkedHashSet<string>();
            var s1 = NewLinkedHashSet<string>();
            var s2 = NewLinkedHashSet<string>();
            var s3 = NewLinkedHashSet<string>();
            var s4 = NewLinkedHashSet<string>();
            var s5 = NewLinkedHashSet<string>();
            s1.UnionWith(Create("foo", "bar", "baz"));
            s2.UnionWith(Create("foo", "bar", "baz", "fooBar"));
            s3.UnionWith(Create("foo", "bar"));
            s4.UnionWith(Create("fooBar", "barBaz", "fooBar"));
            s5.UnionWith(Create("foo", "bar", "baz"));
            Assert.IsTrue(M(s1, s2));
            Assert.IsFalse(M(s1, s3));
            Assert.IsFalse(M(s1, s4));
            Assert.IsFalse(M(s1, s5));

            Assert.IsFalse(M(s1, s1));
            Assert.IsFalse(M(s1, empty));
            Assert.IsTrue(M(empty, s1));
            Assert.IsFalse(M(empty, NewLinkedHashSet<string>()));
        }

        [TestMethod]
        public void IsProperSubsetOf_Null()
        {
            var n = (LinkedHashSet<string>?)null;
            var s = NewLinkedHashSet<string>();
            Assert.ThrowsException<ArgumentNullException>(
                () => _ = s.IsProperSubsetOf(n!));
        }

        [TestMethod]
        public void IsSupersetOf_LinkedHashSet()
        {
            static bool M<T>(LinkedHashSet<T> s1, LinkedHashSet<T> s2)
                    where T : notnull
                => s1.IsSupersetOf(s2);

            var empty = NewLinkedHashSet<string>();
            var s1 = NewLinkedHashSet<string>();
            var s2 = NewLinkedHashSet<string>();
            var s3 = NewLinkedHashSet<string>();
            var s4 = NewLinkedHashSet<string>();
            var s5 = NewLinkedHashSet<string>();
            s1.UnionWith(Create("foo", "bar", "baz"));
            s2.UnionWith(Create("foo", "bar", "baz", "fooBar"));
            s3.UnionWith(Create("foo", "bar"));
            s4.UnionWith(Create("fooBar", "barBaz", "fooBar"));
            s5.UnionWith(Create("foo", "bar", "baz"));
            Assert.IsFalse(M(s1, s2));
            Assert.IsTrue(M(s1, s3));
            Assert.IsFalse(M(s1, s4));
            Assert.IsTrue(M(s1, s5));

            Assert.IsTrue(M(s1, s1));
            Assert.IsTrue(M(s1, empty));
            Assert.IsFalse(M(empty, s1));
            Assert.IsTrue(M(empty, NewLinkedHashSet<string>()));
        }

        [TestMethod]
        public void IsSupersetOf_Null()
        {
            var n = (LinkedHashSet<string>?)null;
            var s = NewLinkedHashSet<string>();
            Assert.ThrowsException<ArgumentNullException>(
                () => _ = s.IsSupersetOf(n!));
        }

        [TestMethod]
        public void IsProperSupersetOf_LinkedHashSet()
        {
            static bool M<T>(LinkedHashSet<T> s1, LinkedHashSet<T> s2)
                    where T : notnull
                => s1.IsProperSupersetOf(s2);

            var empty = NewLinkedHashSet<string>();
            var s1 = NewLinkedHashSet<string>();
            var s2 = NewLinkedHashSet<string>();
            var s3 = NewLinkedHashSet<string>();
            var s4 = NewLinkedHashSet<string>();
            var s5 = NewLinkedHashSet<string>();
            s1.UnionWith(Create("foo", "bar", "baz"));
            s2.UnionWith(Create("foo", "bar", "baz", "fooBar"));
            s3.UnionWith(Create("foo", "bar"));
            s4.UnionWith(Create("fooBar", "barBaz", "fooBar"));
            s5.UnionWith(Create("foo", "bar", "baz"));
            Assert.IsFalse(M(s1, s2));
            Assert.IsTrue(M(s1, s3));
            Assert.IsFalse(M(s1, s4));
            Assert.IsFalse(M(s1, s5));

            Assert.IsFalse(M(s1, s1));
            Assert.IsTrue(M(s1, empty));
            Assert.IsFalse(M(empty, s1));
            Assert.IsFalse(M(empty, NewLinkedHashSet<string>()));
        }

        [TestMethod]
        public void IsProperSupersetOf_Null()
        {
            var n = (LinkedHashSet<string>?)null;
            var s = NewLinkedHashSet<string>();
            Assert.ThrowsException<ArgumentNullException>(
                () => _ = s.IsProperSupersetOf(n!));
        }

        [TestMethod]
        public void SetEquals_LinkedHashSet()
        {
            static bool M<T>(LinkedHashSet<T> s1, LinkedHashSet<T> s2)
                    where T : notnull
                => s1.SetEquals(s2);

            var empty = NewLinkedHashSet<string>();
            var s1 = NewLinkedHashSet<string>();
            var s2 = NewLinkedHashSet<string>();
            var s3 = NewLinkedHashSet<string>();
            var s4 = NewLinkedHashSet<string>();
            var s5 = NewLinkedHashSet<string>();
            s1.UnionWith(Create("foo", "bar", "baz"));
            s2.UnionWith(Create("foo", "bar", "baz", "fooBar"));
            s3.UnionWith(Create("foo", "bar"));
            s4.UnionWith(Create("fooBar", "barBaz", "fooBar"));
            s5.UnionWith(Create("baz", "foo", "bar"));
            Assert.IsFalse(M(s1, s2));
            Assert.IsFalse(M(s1, s3));
            Assert.IsFalse(M(s1, s4));
            Assert.IsTrue(M(s1, s5));

            Assert.IsTrue(M(s1, s1));
            Assert.IsFalse(M(s1, empty));
            Assert.IsFalse(M(empty, s1));
            Assert.IsTrue(M(empty, NewLinkedHashSet<string>()));
        }

        [TestMethod]
        public void SetEquals_Null()
        {
            var n = (LinkedHashSet<string>?)null;
            var s = NewLinkedHashSet<string>();
            Assert.ThrowsException<ArgumentNullException>(
                () => _ = s.SetEquals(n!));
        }

        private static LinkedHashSet<T> NewLinkedHashSet<T>(
                int initialCapacity = 16)
            where T : notnull
        {
            return new LinkedHashSet<T>(initialCapacity);
        }

        private static ImmutableArray<T> Create<T>(params T[] all)
        {
            return ImmutableArray.Create(all);
        }
    }
}

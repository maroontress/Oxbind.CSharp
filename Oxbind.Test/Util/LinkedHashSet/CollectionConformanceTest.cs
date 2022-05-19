namespace Maroontress.Util.LinkedHashSet.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Maroontress.Util;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class CollectionConformanceTest
    {
        [TestMethod]
        public void IsCompliantToCollection()
        {
            IsCompliantToMutableCollection(() => new LinkedHashSet<string>());
        }

        private static void IsCompliantToMutableCollection(
            Func<ISet<string>> newSet)
        {
            {
                var s = newSet();
                Assert.AreEqual(0, s.Count);
                Assert.AreEqual(false, s.IsReadOnly);
                Assert.IsFalse(s.Any());
                Assert.IsFalse(s.Contains("foo"));
                var o = Array.Empty<string>();
                s.CopyTo(o, 0);
                Assert.IsFalse(s.Remove("foo"));
            }
            {
                var s = newSet();
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
            {
                var s = newSet();
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
                Assert.IsTrue(Enumerable.SequenceEqual(
                    new[] { "bar", "foo", }, o));
                Assert.IsFalse(s.Remove("baz"));
                Assert.IsTrue(s.Remove("foo"));
                Assert.IsFalse(s.Remove("foo"));
                Assert.AreEqual(1, s.Count);
                Assert.IsTrue(s.Remove("bar"));
                Assert.IsFalse(s.Remove("bar"));
                Assert.AreEqual(0, s.Count);
            }
            {
                var s = newSet();
                Assert.IsTrue(s.Add("foo"));
                Assert.IsTrue(s.Add("bar"));
                Assert.IsTrue(s.Add("baz"));
                Assert.AreEqual(3, s.Count);
                Assert.IsTrue(s.Any());
                s.Clear();
                Assert.AreEqual(0, s.Count);
                Assert.IsFalse(s.Any());
            }
        }
    }
}

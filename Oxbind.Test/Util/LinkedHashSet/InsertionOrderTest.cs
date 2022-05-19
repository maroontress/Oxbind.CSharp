namespace Maroontress.Util.LinkedHashSet.Test
{
    using System.Linq;
    using Maroontress.Util;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class InsertionOrderTest
    {
        [TestMethod]
        public void AddAndRemove()
        {
            var s = new LinkedHashSet<string>();
            s.Add("foo");
            s.Add("bar");
            s.Add("baz");
            Assert.IsTrue(Enumerable.SequenceEqual(
                new[] { "foo", "bar", "baz", }, s));
            s.Add("bar");
            Assert.IsTrue(Enumerable.SequenceEqual(
                new[] { "foo", "bar", "baz", }, s));
            s.Remove("bar");
            s.Add("bar");
            Assert.IsTrue(Enumerable.SequenceEqual(
                new[] { "foo", "baz", "bar", }, s));
        }

        [TestMethod]
        public void IntersectWith()
        {
            var s = new LinkedHashSet<string>();
            s.Add("foo");
            s.Add("bar");
            s.Add("baz");
            s.IntersectWith(new[] { "baz", "bar", "fooBar", });
            Assert.IsTrue(Enumerable.SequenceEqual(
                new[] { "bar", "baz", }, s));
        }

        [TestMethod]
        public void ExceptWith()
        {
            var s = new LinkedHashSet<string>();
            s.Add("foo");
            s.Add("bar");
            s.Add("baz");
            s.ExceptWith(new[] { "barBaz", "bar", "fooBar", });
            Assert.IsTrue(Enumerable.SequenceEqual(
                new[] { "foo", "baz", }, s));
        }

        [TestMethod]
        public void SymmetricExceptWith()
        {
            /*
                https://sharplab.io/#v2:C4LgTgrgdgPgAgJgIwFgBQcAMACOSAsA3OugG4CGY252AvNlAKYDu2AEuQM4AWAyo8AA8eTAD4AFAEp0Ab3TYF2AEQAzAPZqlAGmUAjStr3kAXtvQBfYmnIA6XgE8Ato4FgAlgGMAogA8PjAAdgAHU3YG5xJmYAbQBdbBlVDQAhAx0lfTBDDMpU03TMpXNJKzwATnERGwApNTcocWydckkS9CA==
            */
            var s = new LinkedHashSet<string>();
            s.Add("foo");
            s.Add("bar");
            s.Add("baz");
            s.SymmetricExceptWith(new[] { "barBaz", "bar", "fooBar", "bar", });
            Assert.IsTrue(Enumerable.SequenceEqual(
                new[] { "foo", "baz", "barBaz", "fooBar" }, s));
        }

        [TestMethod]
        public void UnionWith()
        {
            var s = new LinkedHashSet<string>();
            s.Add("foo");
            s.Add("bar");
            s.Add("baz");
            s.UnionWith(new[] { "barBaz", "bar", "fooBar", });
            Assert.IsTrue(Enumerable.SequenceEqual(
                new[] { "foo", "bar", "baz", "barBaz", "fooBar" }, s));
        }
    }
}

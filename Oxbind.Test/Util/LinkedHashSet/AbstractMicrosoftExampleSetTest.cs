namespace Maroontress.Util.Test
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public abstract class AbstractMicrosoftExampleSetTest
    {
        [TestMethod]
        public void SetEquals()
        {
            /*
                https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.hashset-1.setequals?view=net-6.0#examples
            */
            var lowNumbers = NewSet();
            var allNumbers = NewSet();

            lowNumbers.UnionWith(Enumerable.Range(1, 4));
            allNumbers.UnionWith(Enumerable.Range(0, 10));
            Assert.IsFalse(allNumbers.SetEquals(lowNumbers));
            allNumbers.IntersectWith(lowNumbers);
            Assert.IsTrue(allNumbers.SetEquals(lowNumbers));
        }

        [TestMethod]
        public void UnionWith()
        {
            /*
                https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.hashset-1.unionwith?view=net-6.0#examples
            */
            var evenNumbers = NewSet();
            var oddNumbers = NewSet();

            foreach (var i in Enumerable.Range(0, 5))
            {
                var j = i * 2;
                evenNumbers.Add(j);
                oddNumbers.Add(j + 1);
            }
            var numbers = NewSet();
            numbers.UnionWith(evenNumbers);
            Assert.IsTrue(numbers.SetEquals(evenNumbers));
            numbers.UnionWith(oddNumbers);
            Assert.IsTrue(numbers.SetEquals(Enumerable.Range(0, 10)));
        }

        [TestMethod]
        public void ExceptWith()
        {
            /*
                https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.hashset-1.exceptwith?view=net-6.0
            */
            var lowNumbers = NewSet();
            var highNumbers = NewSet();

            lowNumbers.UnionWith(Enumerable.Range(0, 6));
            highNumbers.UnionWith(Enumerable.Range(3, 7));
            highNumbers.ExceptWith(lowNumbers);
            Assert.IsTrue(highNumbers.SetEquals(Enumerable.Range(6, 4)));
        }

        [TestMethod]
        public void SymmetricExceptWith()
        {
            /*
                https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.hashset-1.symmetricexceptwith?view=net-6.0
            */
            var lowNumbers = NewSet();
            var highNumbers = NewSet();

            lowNumbers.UnionWith(Enumerable.Range(0, 6));
            highNumbers.UnionWith(Enumerable.Range(3, 7));
            lowNumbers.SymmetricExceptWith(highNumbers);
            var numbers = Enumerable.Range(0, 3)
                .Concat(Enumerable.Range(6, 4));
            Assert.IsTrue(lowNumbers.SetEquals(numbers));
        }

        [TestMethod]
        public void Overlaps()
        {
            /*
                https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.hashset-1.overlaps?view=net-6.0#examples
            */
            var lowNumbers = NewSet();
            var allNumbers = NewSet();

            lowNumbers.UnionWith(Enumerable.Range(1, 4));
            allNumbers.UnionWith(Enumerable.Range(0, 10));
            Assert.IsTrue(lowNumbers.Overlaps(allNumbers));
        }

        [TestMethod]
        public void IsSubsetOf()
        {
            /*
                https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.hashset-1.issubsetof?view=net-6.0#examples
            */
            var lowNumbers = NewSet();
            var allNumbers = NewSet();

            lowNumbers.UnionWith(Enumerable.Range(1, 4));
            allNumbers.UnionWith(Enumerable.Range(0, 10));
            Assert.IsTrue(lowNumbers.IsSubsetOf(allNumbers));
            allNumbers.IntersectWith(lowNumbers);
            Assert.IsTrue(lowNumbers.IsSubsetOf(allNumbers));
        }

        [TestMethod]
        public void IsProperSubsetOf()
        {
            /*
                https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.hashset-1.ispropersubsetof?view=net-6.0#examples
            */
            var lowNumbers = NewSet();
            var allNumbers = NewSet();

            lowNumbers.UnionWith(Enumerable.Range(1, 4));
            allNumbers.UnionWith(Enumerable.Range(0, 10));
            Assert.IsTrue(lowNumbers.IsProperSubsetOf(allNumbers));
            allNumbers.IntersectWith(lowNumbers);
            Assert.IsFalse(lowNumbers.IsProperSubsetOf(allNumbers));
        }

        [TestMethod]
        public void IsSupersetOf()
        {
            /*
                https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.hashset-1.issupersetof?view=net-6.0#examples
            */
            var lowNumbers = NewSet();
            var allNumbers = NewSet();

            lowNumbers.UnionWith(Enumerable.Range(1, 4));
            allNumbers.UnionWith(Enumerable.Range(0, 10));
            Assert.IsTrue(allNumbers.IsSupersetOf(lowNumbers));
            allNumbers.IntersectWith(lowNumbers);
            Assert.IsTrue(allNumbers.IsSupersetOf(lowNumbers));
        }

        [TestMethod]
        public void IsProperSupersetOf()
        {
            /*
                https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.hashset-1.ispropersupersetof?view=net-6.0#examples
            */
            var lowNumbers = NewSet();
            var allNumbers = NewSet();

            lowNumbers.UnionWith(Enumerable.Range(1, 4));
            allNumbers.UnionWith(Enumerable.Range(0, 10));
            Assert.IsTrue(allNumbers.IsProperSupersetOf(lowNumbers));
            allNumbers.IntersectWith(lowNumbers);
            Assert.IsFalse(allNumbers.IsProperSupersetOf(lowNumbers));
        }

        protected abstract ISet<int> NewSet();
    }
}

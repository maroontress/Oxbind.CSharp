#pragma warning disable CS1591

namespace Maroontress.Oxbind.Impl.Validator.Test
{
    using System;
    using Maroontress.Oxbind.Impl;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StyleChecker.Annotations;

    [TestClass]
    public sealed class TypeMismatchFromChildTest
    {
        [TestMethod]
        public void RootTest()
        {
            var v = new Validator(typeof(Root));
            Assert.AreEqual(
                "Root: Error: the method annotated with [FromChild] "
                + "must have a single parameter and return void: "
                + "NotifyFirst(First), NotifySecond(Second, Int32)",
                string.Join(Environment.NewLine, v.GetMessages()));
        }

        [ForElement("root")]
        public sealed class Root
        {
            [ElementSchema]
            private static readonly Schema TheSchema = Schema.Of(
                    Mandatory.Of<First>(),
                    Optional.Of<Second>());

            [FromChild]
            private string NotifyFirst([Unused] First first)
            {
                return "";
            }

            [FromChild]
            private void NotifySecond(
                [Unused] Second second, [Unused] int value)
            {
            }
        }

        [ForElement("first")]
        public sealed class First
        {
            [field: ForAttribute("value")]
            public string? Value { get; }
        }

        [ForElement("second")]
        public sealed class Second
        {
            [field: ForAttribute("value")]
            public string? Value { get; }
        }
    }
}

namespace Maroontress.Oxbind.Test.Oxbind.Impl.Validator;

using System;
using System.Collections.Generic;
using Maroontress.Oxbind.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class SameElementNameTest
{
    [TestMethod]
    public void OptionalFollowedByRequired()
    {
        var logger = new Journal("Root");
        var v = Validators.New<OptionalRequiredRoot>(logger);
        Assert.IsFalse(v.IsValid);
        Assert.AreEqual(
            """
            Root: Error: An [Optional] parameter for element 'first' must not be followed by a [Required] or [Multiple] parameter for the same element name: MaybeFirstChild, SecondChild.
            """,
            string.Join(Environment.NewLine, logger.GetMessages()));
    }

    [TestMethod]
    public void OptionalFollowedByMultiple()
    {
        var logger = new Journal("Root");
        var v = Validators.New<OptionalMultipleRoot>(logger);
        Assert.IsFalse(v.IsValid);
        Assert.AreEqual(
            """
            Root: Error: An [Optional] parameter for element 'first' must not be followed by a [Required] or [Multiple] parameter for the same element name: MaybeFirstChild, SecondChildren.
            """,
            string.Join(Environment.NewLine, logger.GetMessages()));
    }

    [TestMethod]
    public void MultipleFollowedByRequired()
    {
        var logger = new Journal("Root");
        var v = Validators.New<MultipleRequiredRoot>(logger);
        Assert.IsFalse(v.IsValid);
        Assert.AreEqual(
            """
            Root: Error: A [Multiple] parameter for element 'first' must not be followed by another parameter for the same element name: FirstChildren, SecondChild.
            """,
            string.Join(Environment.NewLine, logger.GetMessages()));
    }

    [TestMethod]
    public void MultipleFollowedByOptional()
    {
        var logger = new Journal("Root");
        var v = Validators.New<MultipleOptionalRoot>(logger);
        Assert.IsFalse(v.IsValid);
        Assert.AreEqual(
            """
            Root: Error: A [Multiple] parameter for element 'first' must not be followed by another parameter for the same element name: FirstChildren, MaybeSecond.
            """,
            string.Join(Environment.NewLine, logger.GetMessages()));
    }

    [TestMethod]
    public void MultipleFollowedByMultiple()
    {
        var logger = new Journal("Root");
        var v = Validators.New<MultipleMultipleRoot>(logger);
        Assert.IsFalse(v.IsValid);
        Assert.AreEqual(
            """
            Root: Error: A [Multiple] parameter for element 'first' must not be followed by another parameter for the same element name: FirstChildren, SecondChildren.
            """,
            string.Join(Environment.NewLine, logger.GetMessages()));
    }

    [ForElement("root")]
    public record class OptionalRequiredRoot(
        [Optional] First? MaybeFirstChild,
        [Required] Second SecondChild);

    [ForElement("root")]
    public record class OptionalMultipleRoot(
        [Optional] First? MaybeFirstChild,
        [Multiple] IEnumerable<Second> SecondChildren);

    [ForElement("root")]
    public record class MultipleRequiredRoot(
        [Multiple] IEnumerable<First> FirstChildren,
        [Required] Second SecondChild);

    [ForElement("root")]
    public record class MultipleOptionalRoot(
        [Multiple] IEnumerable<First> FirstChildren,
        [Optional] Second? MaybeSecond);

    [ForElement("root")]
    public record class MultipleMultipleRoot(
        [Multiple] IEnumerable<First> FirstChildren,
        [Multiple] IEnumerable<Second> SecondChildren);

    [ForElement("first")]
    public record class First;

    [ForElement("first")]
    public record class Second;
}

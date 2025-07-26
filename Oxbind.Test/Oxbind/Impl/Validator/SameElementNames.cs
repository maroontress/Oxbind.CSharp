namespace Maroontress.Oxbind.Test.Oxbind.Impl.Validator;

using System;
using Maroontress.Oxbind.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

public static class SameElementNames
{
    public static void OptionalFollowedByRequired<T>()
        => Check<T>(
            """
            Error: An [Optional] parameter for element 'first' must not be followed by a [Required] or [Multiple] parameter for the same element name: MaybeFirstChild, SecondChild.
            """);

    public static void OptionalFollowedByMultiple<T>()
        => Check<T>(
            """
            Error: An [Optional] parameter for element 'first' must not be followed by a [Required] or [Multiple] parameter for the same element name: MaybeFirstChild, SecondChildren.
            """);

    public static void MultipleFollowedByRequired<T>()
        => Check<T>(
            """
            Error: A [Multiple] parameter for element 'first' must not be followed by another parameter for the same element name: FirstChildren, SecondChild.
            """);

    public static void MultipleFollowedByOptional<T>()
        => Check<T>(
            """
            Error: A [Multiple] parameter for element 'first' must not be followed by another parameter for the same element name: FirstChildren, MaybeSecond.
            """);

    public static void MultipleFollowedByMultiple<T>()
        => Check<T>(
            """
            Error: A [Multiple] parameter for element 'first' must not be followed by another parameter for the same element name: FirstChildren, SecondChildren.
            """);

    private static void Check<T>(string expected)
    {
        var logger = new Journal("Root");
        var v = Validators.New<T>(logger);
        Assert.IsFalse(v.IsValid);
        Assert.AreEqual(
            $"Root: {expected}",
            string.Join(Environment.NewLine, logger.GetMessages()));
    }
}

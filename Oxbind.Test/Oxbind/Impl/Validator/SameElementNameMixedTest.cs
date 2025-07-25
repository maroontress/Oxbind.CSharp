namespace Maroontress.Oxbind.Test.Oxbind.Impl.Validator;

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class SameElementNameMixedTest
{
    [TestMethod]
    public void OptionalFollowedByRequired()
        => SameElementNames.OptionalFollowedByRequired<OptionalRequiredRoot>();

    [TestMethod]
    public void OptionalFollowedByMultiple()
        => SameElementNames.OptionalFollowedByMultiple<OptionalMultipleRoot>();

    [TestMethod]
    public void MultipleFollowedByRequired()
        => SameElementNames.MultipleFollowedByRequired<MultipleRequiredRoot>();

    [TestMethod]
    public void MultipleFollowedByOptional()
        => SameElementNames.MultipleFollowedByOptional<MultipleOptionalRoot>();

    [TestMethod]
    public void MultipleFollowedByMultiple()
        => SameElementNames.MultipleFollowedByMultiple<MultipleMultipleRoot>();

    [ForElement("root")]
    public record class OptionalRequiredRoot(
        [Optional] First? MaybeFirstChild,
        [Required] BindResult<Second> SecondChild);

    [ForElement("root")]
    public record class OptionalMultipleRoot(
        [Optional] First? MaybeFirstChild,
        [Multiple] IEnumerable<BindResult<Second>> SecondChildren);

    [ForElement("root")]
    public record class MultipleRequiredRoot(
        [Multiple] IEnumerable<First> FirstChildren,
        [Required] BindResult<Second> SecondChild);

    [ForElement("root")]
    public record class MultipleOptionalRoot(
        [Multiple] IEnumerable<First> FirstChildren,
        [Optional] BindResult<Second>? MaybeSecond);

    [ForElement("root")]
    public record class MultipleMultipleRoot(
        [Multiple] IEnumerable<First> FirstChildren,
        [Multiple] IEnumerable<BindResult<Second>> SecondChildren);

    [ForElement("first")]
    public record class First;

    [ForElement("first")]
    public record class Second;
}

namespace Maroontress.Oxbind.Test.Oxbind.Impl.Validator;

using System;
using System.Collections.Generic;
using Maroontress.Oxbind.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class MissingForChildElementTest
{
    [TestMethod]
    public void StringWithoutForChildElement()
        => Check<RequiredString>("Name");

    [TestMethod]
    public void BindResultStringWithoutForChildElement()
        => Check<RequiredBindResultString>("Name");

    [TestMethod]
    public void IEnumerableStringWithoutForChildElement()
        => Check<MultipleString>("Names");

    [TestMethod]
    public void IEnumerableBindResultStringWithoutForChildElement()
        => Check<MultipleBindResultString>("Names");

    private static void Check<T>(string propertyName)
    {
        var logger = new Journal("Root");
        var v = Validators.New<T>(logger);
        Assert.IsFalse(v.IsValid);
        Assert.AreEqual(
            $"""
            Root: Error: A constructor parameter for a string child element must be annotated with [ForChildElement]: {propertyName}.
            """,
            string.Join(Environment.NewLine, logger.GetMessages()));
    }

    [ForElement("root")]
    private record class RequiredString(
        /* Missing [ForChildElement] */
        [Required] string Name);

    [ForElement("root")]
    private record class RequiredBindResultString(
        /* Missing [ForChildElement] */
        [Required] BindResult<string> Name);

    [ForElement("root")]
    private record class MultipleString(
        /* Missing [ForChildElement] */
        [Multiple] IEnumerable<string> Names);

    [ForElement("root")]
    private record class MultipleBindResultString(
        /* Missing [ForChildElement] */
        [Multiple] IEnumerable<BindResult<string>> Names);
}

namespace Maroontress.Oxbind.Test;

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Provides check methods shared with various tests.
/// </summary>
public static class Checks
{
    public static void ThrowBindException(
        Action action,
        string actionLabel,
        string expectedExceptionMessage)
    {
        try
        {
            action();
        }
        catch (BindException e)
        {
            Assert.AreEqual(expectedExceptionMessage, e.GetFullMessage());
            return;
        }
        catch (Exception e)
        {
            Assert.Fail(e.ToString());
        }
        Assert.Fail($"{actionLabel} did not throw any exception.");
    }
}

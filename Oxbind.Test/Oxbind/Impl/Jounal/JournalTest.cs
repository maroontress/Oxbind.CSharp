namespace Maroontress.Oxbind.Test.Oxbind.Impl.Jounal;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using Maroontress.Oxbind.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StyleChecker.Annotations;

[TestClass]
public class JournalTest
{
    [TestMethod]
    public void Warn_AddsWarningMessage()
    {
        var journal = new Journal(
            "TestLabel",
            new DecoyResourceManager(),
            CultureInfo.InvariantCulture);
        journal.Warn("test_message", 123);
        var messages = journal.GetMessages()
            .ToList();
        Assert.AreEqual(1, messages.Count);
        StringAssert.Contains(
            messages[0], "TestLabel: Warning: Test message: 123");
        Assert.IsFalse(journal.HasError);
    }

    [TestMethod]
    public void Error_AddsErrorMessageAndSetsHasError()
    {
        var journal = new Journal(
            "TestLabel",
            new DecoyResourceManager(),
            CultureInfo.InvariantCulture);
        journal.Error("test_message", "abc");
        var messages = journal.GetMessages()
            .ToList();
        Assert.AreEqual(1, messages.Count);
        StringAssert.Contains(
            messages[0], "TestLabel: Error: Test message: abc");
        Assert.IsTrue(journal.HasError);
    }

    [TestMethod]
    public void Error_CalledMultipleTimes()
    {
        var journal = new Journal(
            "TestLabel",
            new DecoyResourceManager(),
            CultureInfo.InvariantCulture);
        journal.Error("test_message", 1);
        journal.Error("test_message", 2);
        var messages = journal.GetMessages()
            .ToList();
        Assert.AreEqual(2, messages.Count);
        StringAssert.Contains(
            messages[0], "TestLabel: Error: Test message: 1");
        StringAssert.Contains(
            messages[1], "TestLabel: Error: Test message: 2");
        Assert.IsTrue(journal.HasError);
    }

    [TestMethod]
    public void GetMessages_ReturnsAllMessages()
    {
        var journal = new Journal(
            "TestLabel",
            new DecoyResourceManager(),
            CultureInfo.InvariantCulture);
        journal.Warn("test_message", "foo");
        journal.Error("test_message", "bar");
        var messages = journal.GetMessages()
            .ToList();
        Assert.AreEqual(2, messages.Count);
        StringAssert.Contains(messages[0], "Warning: Test message: foo");
        StringAssert.Contains(messages[1], "Error: Test message: bar");
    }

    [TestMethod]
    public void Log_ThrowsIfResourceKeyNotFound()
    {
        var journal = new Journal(
            "TestLabel",
            new DecoyResourceManager(),
            CultureInfo.InvariantCulture);
        var exception = Assert.ThrowsExactly<ArgumentException>(
            () => journal.Warn("not_found_key", 1));
        StringAssert.Contains(
            exception.Message, "Resource key 'not_found_key' not found.");
    }

    private sealed class DecoyResourceManager
        : ResourceManager
    {
        private Dictionary<string, string> Resources { get; } = new()
        {
            ["warning"] = "Warning",
            ["error"] = "Error",
            ["test_message"] = "Test message: {0}",
        };

        public override string? GetString(
                string key, [Unused] CultureInfo? culture)
            => Resources.TryGetValue(key, out var value)
                ? value
                : null;
    }
}

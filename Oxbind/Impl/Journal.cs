namespace Maroontress.Oxbind.Impl;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;

/// <summary>
/// Provides a logging mechanism, primarily for validation messages.
/// </summary>
public sealed class Journal
{
    /// <summary>
    /// An action to record an error message.
    /// </summary>
    private Action<string, object[]> errorAction;

    /// <summary>
    /// Initializes a new instance of the <see cref="Journal"/>
    /// class.
    /// </summary>
    /// <param name="label">
    /// The prefix for each log message.
    /// </param>
    /// <param name="maybeManager">
    /// The resource manager to use for formatting messages, or <c>null</c> to
    /// use the default.
    /// </param>
    /// <param name="maybeCulture">
    /// The culture to use for formatting messages, or <c>null</c> to use the
    /// current culture.
    /// </param>
    public Journal(
        string label,
        ResourceManager? maybeManager = null,
        CultureInfo? maybeCulture = null)
    {
        var manager = maybeManager ?? Resource.ResourceManager;
        var culture = maybeCulture ?? Resource.Culture;

        Label = label;
        Bundle = s => manager.GetString(s, culture);

        void NonFirstErrorAction(string m, object[] a)
            => Log("error", m, a);

        errorAction = (m, a) =>
        {
            NonFirstErrorAction(m, a);
            HasError = true;
            errorAction = NonFirstErrorAction;
        };
    }

    /// <summary>
    /// Gets a value indicating whether this logger has recorded any errors.
    /// </summary>
    public bool HasError { get; private set; } = false;

    /// <summary>
    /// Gets the prefix for each log message.
    /// </summary>
    private string Label { get; }

    /// <summary>
    /// Gets the resource bundle.
    /// </summary>
    private Func<string, string?> Bundle { get; }

    /// <summary>
    /// Gets the buffer to store log messages.
    /// </summary>
    private List<string> Messages { get; } = [];

    /// <summary>
    /// Gets the collection of log messages representing the warnings and
    /// errors recorded by this logger.
    /// </summary>
    /// <returns>
    /// The log messages.
    /// </returns>
    public IEnumerable<string> GetMessages()
        => Messages;

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    /// <param name="message">
    /// The resource key for the format string pattern.
    /// </param>
    /// <param name="args">
    /// The arguments for the formatted string.
    /// </param>
    public void Warn(string message, params object[] args)
        => Log("warning", message, args);

    /// <summary>
    /// Logs an error message and marks this logger to indicate that an error
    /// has been recorded.
    /// </summary>
    /// <param name="message">
    /// The resource key for the format string pattern.
    /// </param>
    /// <param name="args">
    /// The arguments for the formatted string.
    /// </param>
    public void Error(string message, params object[] args)
        => errorAction(message, args);

    /// <summary>
    /// Appends a log message to the buffer.
    /// </summary>
    /// <param name="type">
    /// <c>"warning"</c> or <c>"error"</c>.
    /// </param>
    /// <param name="message">
    /// The resource key for the format string pattern.
    /// </param>
    /// <param name="args">
    /// The arguments for the formatted string.
    /// </param>
    private void Log(string type, string message, params object[] args)
    {
        var culture = CultureInfo.CurrentCulture;
        if (Bundle(message) is not {} format)
        {
            throw new ArgumentException(
                $"Resource key '{message}' not found.", nameof(message));
        }
        var m = string.Format(culture, format, args);
        Messages.Add($"{Label}: {Bundle(type)}: {m}");
    }
}

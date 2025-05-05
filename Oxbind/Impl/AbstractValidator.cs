namespace Maroontress.Oxbind.Impl;

using System;
using System.Collections.Generic;
using System.Globalization;

/// <summary>
/// An abstraction of the validator.
/// </summary>
public abstract class AbstractValidator
{
    /// <summary>
    /// Indicates whether this validator has detected errors.
    /// </summary>
    private bool hasError = false;

    /// <summary>
    /// The action to record an error message.
    /// </summary>
    private Action<string, object[]> errorAction;

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractValidator"/>
    /// class.
    /// </summary>
    /// <param name="label">
    /// The prefix for each log message.
    /// </param>
    protected AbstractValidator(string label)
    {
        var manager = Resource.ResourceManager;
        var culture = Resource.Culture;

        Label = label;
        Bundle = s => manager.GetString(s, culture);

        void NonFirstErrorAction(string m, object[] a)
            => Log("error", m, a);

        errorAction = (m, a) =>
        {
            NonFirstErrorAction(m, a);
            hasError = true;
            errorAction = NonFirstErrorAction;
        };
    }

    /// <summary>
    /// Gets a value indicating whether this validator has detected errors.
    /// </summary>
    /// <returns>
    /// <c>true</c> if this validator has detected errors.
    /// </returns>
    public bool IsValid => !hasError;

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
    /// Get a new log messages representing the warnings/errors that this
    /// validator has detected.
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
    /// The key of the resource bundle representing the pattern of the
    /// formatted string.
    /// </param>
    /// <param name="args">
    /// The arguments for the formatted string.
    /// </param>
    protected void Warn(string message, params object[] args)
        => Log("warning", message, args);

    /// <summary>
    /// Logs an error message and marks this validator to indicate that it has
    /// detected errors.
    /// </summary>
    /// <param name="message">
    /// The key of the resource bundle representing the pattern of the
    /// formatted string.
    /// </param>
    /// <param name="args">
    /// The arguments for the formatted string.
    /// </param>
    protected void Error(string message, params object[] args)
        => errorAction(message, args);

    /// <summary>
    /// Appends a log message to the buffer.
    /// </summary>
    /// <param name="type">
    /// <c>"warning"</c> or <c>"error"</c>.
    /// </param>
    /// <param name="message">
    /// The key of the resource bundle representing the pattern of the
    /// formatted string.
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
                $"unknown key: {message}", nameof(message));
        }
        var m = string.Format(culture, format, args);
        Messages.Add($"{Label}: {Bundle(type)}: {m}");
    }
}

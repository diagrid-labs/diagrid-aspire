using System;
using System.Collections.Generic;

namespace Diagrid.Aspire.Hosting;

/// <summary>Renamed to <see cref="DiagridCliException"/>. Kept for one minor release so that an existing
/// <c>catch (CatalystCliException)</c> still catches what the library throws.</summary>
[Obsolete("Renamed to DiagridCliException. Catalyst is now Diagrid; this type is removed in the next minor.")]
public class CatalystCliException : Exception
{
    public CatalystCliException(
        IReadOnlyList<string> arguments,
        int exitCode,
        string stdErr,
        string? message = null,
        Exception? innerException = null
    )
        : base(message ?? BuildMessage(arguments, exitCode, stdErr), innerException)
    {
        Arguments = arguments;
        ExitCode = exitCode;
        StdErr = stdErr;
    }

    public IReadOnlyList<string> Arguments { get; }

    /// <summary>
    /// The CLI's exit code. Can be 0 when the CLI succeeded but its output was cut off and could not be read;
    /// <see cref="Exception.Message"/> says so in that case.
    /// </summary>
    public int ExitCode { get; }

    /// <summary>
    /// The CLI's stderr after cleaning (escape sequences and the update notice removed), NOT verbatim output.
    /// </summary>
    public string StdErr { get; }

    internal static string BuildMessage(
        IReadOnlyList<string> arguments,
        int exitCode,
        string stdErr,
        bool outputTruncated = false
    )
    {
        var message = $"'diagrid {string.Join(" ", arguments)}' failed with exit code {exitCode}.";
        if (!string.IsNullOrWhiteSpace(stdErr))
        {
            message += $" {stdErr.Trim()}";
        }

        if (outputTruncated)
        {
            message += " (The CLI's output was cut off, so the error text above may be incomplete.)";
        }

        return message;
    }
}

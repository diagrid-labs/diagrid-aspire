using System;
using System.Collections.Generic;

namespace Diagrid.Aspire.Hosting;

/// <summary>Raised when the <c>diagrid</c> CLI fails, times out, or cannot be run.</summary>
// Derives from the obsolete CatalystCliException for one minor release, so an existing
// catch (CatalystCliException) still catches this. CS0618 is suppressed only here, at the one site where
// deriving from an obsolete base is the entire point; when the base goes, its members move back in here.
#pragma warning disable CS0618
public sealed class DiagridCliException : CatalystCliException
#pragma warning restore CS0618
{
    public DiagridCliException(
        IReadOnlyList<string> arguments,
        int exitCode,
        string stdErr,
        string? message = null,
        Exception? innerException = null
    )
        : base(arguments, exitCode, stdErr, message, innerException)
    {
    }
}

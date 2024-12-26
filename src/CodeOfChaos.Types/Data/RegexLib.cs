// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace
namespace System.Text.RegularExpressions;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static partial class CommonRegexLib {

    private const RegexOptions DefaultOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;

    [GeneratedRegex(@"^(\d+)\.(\d+)\.(\d+)(?:\-(\w*))?$", DefaultOptions)]
    public static partial Regex SemanticVersion { get; }
}

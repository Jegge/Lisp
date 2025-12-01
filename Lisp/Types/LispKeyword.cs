using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Lisp.Types;

[DebuggerDisplay("Keyword {Value}")]
public sealed partial class LispKeyword (string value) : LispValue
{
    public LispKeyword (Enum value) : this (ToKebapCase(value.ToString()))  { }

    internal const char Token = ':';

    public string Value { get; } = value;
    public override bool Equals(object? obj) => obj is LispKeyword keyword && Value == keyword.Value;
    public override int GetHashCode() => HashCode.Combine(Value);
    public override string Print (bool readable) => $"{Token}{Value}";

    private static string ToKebapCase (string text) => KebapCaseRegex().Replace(text, "-$1").ToLowerInvariant();

    [GeneratedRegex(@"(?<!^)(?<!-)((?<=\p{Ll})\p{Lu}|\p{Lu}(?=\p{Ll}))")]
    private static partial Regex KebapCaseRegex();
}
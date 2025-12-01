using System.Diagnostics;

namespace Lisp.Types;

[DebuggerDisplay("String \"{Value}\"")]
public sealed class LispString (string value) : LispValue
{
    internal struct Token
    {
        public const char Delimiter = '"';
    }
    public string Value { get; } = value;
    public override bool Equals (object? obj) => obj is LispString other && Value == other.Value;
    public override int GetHashCode() => HashCode.Combine(Value);

    public override string Print (bool readable) =>
        readable ? $"{Token.Delimiter}{Escape(Value)}{Token.Delimiter}" : Value;
    public static string Escape (string text) =>
        string.Join(string.Empty, text.Select(c => c switch
        {
            '\"' => "\\\"",
            '\r' => "\\r",
            '\n' => "\\n",
            '\t' => "\\t",
            '\\' => @"\\",
            _ => c.ToString()
        }));
}
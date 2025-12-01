using System.Diagnostics;

namespace Lisp.Types;

[DebuggerDisplay("Bool {Value}")]
public sealed class LispBool (bool value) : LispValue
{
    internal struct Token
    {
        public const string True = "true";
        public const string False = "false";
    }

    public bool Value { get; } = value;
    public override bool Equals (object? obj) => obj is LispBool other && Value == other.Value;
    public override int GetHashCode() => HashCode.Combine(Value);
    public override string Print (bool readable) => Value ? Token.True : Token.False;
}
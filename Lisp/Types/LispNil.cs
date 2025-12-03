using System.Diagnostics;

namespace Lisp.Types;

[DebuggerDisplay("Nil")]
public sealed class LispNil : LispValue
{
    internal LispNil() { }

    internal const string Token = "nil";

    public override bool Equals (object? obj) => ReferenceEquals(obj, Nil) || obj is LispNil;
    public override int GetHashCode () => 0;
    public override string Print (bool readable) => Token;
}
using System.Diagnostics;

namespace Lisp.Types;

[DebuggerDisplay("Atom {Value}")]
public sealed class LispAtom (LispValue value) : LispValue
{
    private const string Token = "atom";
    public LispValue Value { get; set; } = value;
    public override string Print (bool readable) => $"{LispList.Token.Begin}{Token} {Value.Print(readable)}{LispList.Token.End}";
}
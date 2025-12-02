using System.Diagnostics;

namespace Lisp.Types;

[DebuggerDisplay("Function macro: {IsMacro}")]
public sealed class LispLambda (IEnumerable<LispSymbol> bindings, LispSymbol? varArg, LispValue body, LispEnvironment environment) : LispApplicable
{
    public const string Token = "lambda";

    public bool IsMacro { get; internal set; }
    public LispSymbol[] Arguments { get; } = bindings.ToArray();
    public LispSymbol? VarArg { get; } = varArg;
    public LispEnvironment Environment { get; } = environment;
    public LispValue Body { get; } = body;
    public override bool Equals(object? obj) => obj is LispLambda other && Body == other.Body;
    public override int GetHashCode() => HashCode.Combine(Body);

    public override string Print (bool readable) =>
        VarArg is null
            ? $"{LispList.Token.Begin}{Token} ({string.Join(' ', Arguments.Select(b => b.Print(readable)))}) {Body.Print(readable)}{LispList.Token.End}"
            : $"{LispList.Token.Begin}{Token} ({string.Join(' ', Arguments.Select(b => b.Print(readable)))} {LispDotList.Token.Dot} {VarArg.Print(readable)}) {Body.Print(readable)}{LispList.Token.End}";
}
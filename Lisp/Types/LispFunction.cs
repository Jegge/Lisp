using System.Diagnostics;

namespace Lisp.Types;

[DebuggerDisplay("Function macro: {IsMacro}")]
public sealed class LispFunction (IEnumerable<LispSymbol> bindings, LispValue body, LispEnvironment environment) : LispApplicable
{
    internal struct Token
    {
        public const string Lambda = "lambda";
        public const string DefineMacro = "define-macro";
        public const string Variadic = "&";
    }

    public bool IsMacro { get; internal set; }
    public LispSymbol[] Bindings { get; } = bindings.ToArray();
    public LispEnvironment Environment { get; } = environment;
    public LispValue Body { get; } = body;
    public override bool Equals(object? obj) => obj is LispFunction other && Body == other.Body;
    public override int GetHashCode() => HashCode.Combine(Body);

    public override string Print (bool readable) =>
        $"{LispList.Token.Begin}{Token.Lambda} ({string.Join(' ', Bindings.Select(b => b.Print(readable)))}) {Body.Print(readable)}{LispList.Token.End}";
}
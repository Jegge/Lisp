using System.Diagnostics;

namespace Lisp.Types;

[DebuggerDisplay("Symbol {Value}")]
public sealed class LispSymbol (string value) : LispValue
{
    internal struct Token
    {
        internal const string Quote = "quote";
        internal const string Quasiquote = "quasiquote";
        internal const string Unquote = "unquote";
        internal const string SpliceUnquote = "splice-unquote";
        internal const string Deref = "deref";
        internal const string If = "if";
        internal const string Do = "do";
        internal const string Let = "let";
        internal const string Define = "define";
        internal const string Try = "try";
        internal const string Catch = "catch";
    }

    public string Value { get; } = value;
    public override bool Equals (object? obj) => obj is LispSymbol symbol && Value == symbol.Value;
    public override int GetHashCode () => HashCode.Combine(Value);

    internal override LispValue QuasiQuoteUnquoted () => new LispList(new LispSymbol(Token.Quote), this);
    public override string Print (bool readable) => Value;

    public static readonly LispSymbol Concat = new ("concat");
    public static readonly LispSymbol Cons = new ("cons");

    public static readonly LispSymbol Vec = new ("vec");
    public static readonly LispSymbol DebugEval = new ("DEBUG-EVAL");
}
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
        internal const string DefineMacro = "define-macro";
        internal const string Try = "try";
        internal const string Catch = "catch";
        internal const string DebugEval = "*debug-eval*";
    }

    public string Value { get; } = value;
    public override bool Equals (object? obj) => obj is LispSymbol symbol && Value == symbol.Value;
    public override int GetHashCode () => HashCode.Combine(Value);

    public bool IsBuiltIn => 
        Value == Token.Try ||
        Value == Token.Catch ||
        Value == Token.Quote ||
        Value == Token.Quasiquote ||
        Value == Token.Do ||
        Value == Token.Let ||
        Value == Token.If ||
        Value == Token.Define ||
        Value == Token.DefineMacro ||
        Value == LispLambda.Token;

    internal override LispValue QuasiQuoteUnquoted () => new LispList(new LispSymbol(Token.Quote), this);
    public override string Print (bool readable) => Value;

    public static readonly LispSymbol Concat = new ("concat");
    public static readonly LispSymbol Cons = new ("cons");

    public static readonly LispSymbol Vec = new ("vec");
    public static readonly LispSymbol DebugEval = new (Token.DebugEval);
}
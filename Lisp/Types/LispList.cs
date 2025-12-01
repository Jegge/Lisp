using System.Diagnostics;

namespace Lisp.Types;

[DebuggerDisplay("List ({Count} elements)")]
public sealed class LispList (IEnumerable<LispValue> value) : LispSequential (value)
{
    public LispList() : this ([]) { }

    internal struct Token
    {
        public const string Begin = "(";
        public const string End = ")";
    }
    public LispList (params LispValue[] values) : this(values.AsEnumerable()) { }

    internal override LispValue QuasiQuoteUnquoted () =>
        Values switch
        {
            [LispSymbol { Value: LispSymbol.Token.Unquote }, { } unquoted] =>
                unquoted, _ =>
                Values.Reverse().Aggregate(new LispList(), (current, element) =>
                    element is LispList { Values: [LispSymbol { Value: LispSymbol.Token.SpliceUnquote }, { } spliceUnquoted] }
                        ? new LispList(LispSymbol.Concat, spliceUnquoted, current)
                        : new LispList(LispSymbol.Cons, element.QuasiQuoteUnquoted(), current))
        };

    public override string Print (bool readable) =>
        $"{Token.Begin}{string.Join(' ', Values.Select(v => v.Print(readable)))}{Token.End}";
}
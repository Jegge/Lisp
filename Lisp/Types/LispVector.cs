using System.Diagnostics;

namespace Lisp.Types;

[DebuggerDisplay("Vector ({Count} elements)")]
public sealed class LispVector(IEnumerable<LispValue> value) : LispSequential(value)
{
    public LispVector () : this([]) {}

    internal struct Token
    {
        public const string Begin = "[";
        public const string End = "]";
    }
    internal override LispValue QuasiQuoteUnquoted ()
    {
        var accumulator = Values.Reverse()
            .Aggregate(new LispList(), (current, element) =>
                element is LispList { Values: [LispSymbol { Value: LispSymbol.Token.SpliceUnquote }, { } spliceUnquoted] }
                    ? new LispList(LispSymbol.Concat, spliceUnquoted, current)
                    : new LispList(LispSymbol.Cons, element.QuasiQuoteUnquoted(), current));
        return new LispList(LispSymbol.Vec, accumulator);
    }

    public override string Print (bool readable) =>
        $"{Token.Begin}{string.Join(' ', Values.Select(v => v.Print(readable)))}{Token.End}";
}
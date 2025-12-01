using System.Diagnostics;

namespace Lisp.Types;

[DebuggerDisplay("HashMap ({Values.Count} elements)")]
public sealed class LispHashMap(IDictionary<LispValue,LispValue> value) : LispContainer
{
    public LispHashMap () : this(new Dictionary<LispValue, LispValue>()) { }

    internal struct Token
    {
        public const string Begin = "{";
        public const string End = "}";
    }
    public IDictionary<LispValue,LispValue> Values { get; } = value;
    public int Count => Values.Count;
    public override bool Equals(object? obj)
        => obj is LispHashMap other &&
           Values.Count == other.Values.Count &&
           Values.Keys.Zip(other.Values.Keys).All(v => v.First.Equals(v.Second)) &&
           Values.Keys.All(k => Values[k].Equals(other.Values[k]));
    public override int GetHashCode() => HashCode.Combine(Values);
    internal override LispValue QuasiQuoteUnquoted () => new LispList(new LispSymbol(LispSymbol.Token.Quote), this);
    public override string Print (bool readable) =>
        $"{Token.Begin}{string.Join(' ', Values.Select(v => $"{v.Key.Print(readable)} {v.Value.Print(readable)}"))}{Token.End}";
}
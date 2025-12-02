using System.Diagnostics;

namespace Lisp.Types;

[DebuggerDisplay("DotList ({Count} elements)")]
public sealed class LispDotList (IEnumerable<LispValue> head, LispValue tail) : LispSequential(head.Append(tail))
{
    internal struct Token
    {
        public const string Dot = ".";
    }

    public LispValue[] Head => Values[..^1];
    public LispValue Tail => Values.Last();

    public override string Print (bool readable) =>
        $"{LispList.Token.Begin}{string.Join(' ', Values.SkipLast(1).Select(v => v.Print(readable)))} {Token.Dot} {Values.Last().Print(readable)}{LispList.Token.End}";
}
using System.Diagnostics;

namespace Lisp.Parser;

[DebuggerDisplay("({Line}, {Column}) {Value}")]
internal readonly struct LispToken (int line, int column, string value)
{
    public int Line { get; } = line;
    public int Column { get; } = column;
    public string Value { get; } = value;
}

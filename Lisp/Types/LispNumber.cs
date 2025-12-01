using System.Diagnostics;
using System.Globalization;

namespace Lisp.Types;

[DebuggerDisplay("Number {Value}")]
public sealed class LispNumber (decimal value) : LispValue
{
    public decimal Value { get; } = value;
    public override bool Equals(object? obj) => obj is LispNumber other && Value == other.Value;
    public override int GetHashCode() => HashCode.Combine(Value);
    public override string Print (bool readable) => Value.ToString(CultureInfo.InvariantCulture);
}
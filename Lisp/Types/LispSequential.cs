namespace Lisp.Types;

public abstract class LispSequential (IEnumerable<LispValue> values) : LispContainer
{
    public LispValue[] Values { get; } = values.ToArray();
    public int Count => Values.Length;
    public override bool Equals(object? obj)
        => obj is LispSequential other &&
           Values.Length == other.Values.Length &&
           Values.Zip(other.Values).All(v => v.First.Equals(v.Second));

    public override int GetHashCode() => HashCode.Combine(Values);

    public T GetAt<T> (int index) where T : LispValue
    {
        if (index >= Values.Length)
            throw new ArgumentCountException(index + 1, Values.Length);
        return Values[index] as T ?? throw new TypeMismatchException<T>(Values[index]);
    }
    internal IEnumerable<(T1, T2)> TuplesOf<T1, T2> () where T1 : LispValue where T2 : LispValue
    {
        if (Values.Length == 0 || Values.Length % 2 == 1)
            throw new RuntimeException("Expected even number of arguments");
        for (var i = 0; i < Values.Length; i += 2)
            yield return (GetAt<T1>(i), GetAt<T2>(i + 1));
    }

    internal IEnumerable<T> As<T> () where T : LispValue
    {
        foreach (var value in Values)
            if (value is not T casted)
                throw new TypeMismatchException<T>(value);
            else
                yield return casted;
    }
}
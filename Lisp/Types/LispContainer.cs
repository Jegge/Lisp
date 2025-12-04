namespace Lisp.Types;

public abstract class LispContainer : LispValue
{
    public abstract int Count { get; }
    public abstract bool Contains(LispValue value);
}
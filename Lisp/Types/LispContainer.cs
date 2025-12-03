namespace Lisp.Types;

public abstract class LispContainer : LispValue
{
    public abstract int Count { get; }
}
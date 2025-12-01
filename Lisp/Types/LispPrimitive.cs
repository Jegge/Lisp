using System.Diagnostics;

namespace Lisp.Types;

[DebuggerDisplay("Primitive {Name}")]
public sealed class LispPrimitive (string name, Func<LispEnvironment, LispList, LispValue> body) : LispApplicable
{
    public string Name { get; } = name;
    public Func<LispEnvironment, LispList, LispValue> Body { get; } = body;
    public override bool Equals(object? obj) => obj is LispPrimitive other && Name == other.Name;
    public override int GetHashCode() => HashCode.Combine(Name);
    public override string Print (bool readable) => $"<primitive {Name}>";

    internal static LispPrimitive Define (string name, Func<bool, bool, bool> body) =>
        new (name, (_, arguments) =>
            arguments.Count != 2
                ? throw new ArgumentCountException(2, arguments.Count, name)
                : new LispBool(body(arguments.GetAt<LispBool>(0).Value, arguments.GetAt<LispBool>(1).Value)));

    internal static LispPrimitive Define (string name, Func<decimal, decimal, bool> body) =>
        new (name, (_, arguments) =>
            arguments.Count != 2
                ? throw new ArgumentCountException(2, arguments.Count, name)
                : new LispBool(body(arguments.GetAt<LispNumber>(0).Value, arguments.GetAt<LispNumber>(1).Value)));

    internal static LispPrimitive Define (string name, Func<decimal, decimal, decimal> body) =>
        new (name, (_, arguments) =>
            arguments.Count != 2
                ? throw new ArgumentCountException(2, arguments.Count, name)
                : new LispNumber(body(arguments.GetAt<LispNumber>(0).Value, arguments.GetAt<LispNumber>(1).Value)));

    internal static LispPrimitive Define<TResult> (string name, Func<LispEnvironment, TResult> body)
        where TResult : LispValue =>
        new (name, (environment, _) => body(environment));

    internal static LispPrimitive Define<T1, TResult> (string name, Func<LispEnvironment, T1, TResult> body)
        where T1 : LispValue where TResult : LispValue =>
        new (name, (environment, arguments) =>
            arguments.Count != 1
                ? throw new ArgumentCountException(1, arguments.Count, name)
                : body(environment, arguments.GetAt<T1>(0)));

    internal static LispPrimitive Define<T1, T2, TResult> (string name, Func<LispEnvironment, T1, T2, TResult> body)
        where T1 : LispValue where T2 : LispValue where TResult : LispValue =>
        new (name, (environment, arguments) =>
            arguments.Count != 2
                ? throw new ArgumentCountException(2, arguments.Count, name)
                : body(environment, arguments.GetAt<T1>(0), arguments.GetAt<T2>(1)));

    internal static LispPrimitive Define<T1, T2, T3, TResult> (string name, Func<LispEnvironment, T1, T2, T3, TResult> body)
        where T1 : LispValue where T2 : LispValue where T3 : LispValue where TResult : LispValue =>
        new (name, (environment, arguments) =>
            arguments.Count != 3
                ? throw new ArgumentCountException(3, arguments.Count, name)
                : body(environment, arguments.GetAt<T1>(0), arguments.GetAt<T2>(1), arguments.GetAt<T3>(2)));

    internal static LispPrimitive Define<T1, T2, T3, T4, TResult> (string name, Func<LispEnvironment, T1, T2, T3, T4, TResult> body)
        where T1 : LispValue where T2 : LispValue where T3 : LispValue where T4 : LispValue where TResult : LispValue =>
        new (name, (environment, arguments) =>
            arguments.Count != 4
                ? throw new ArgumentCountException(4, arguments.Count, name)
                : body(environment, arguments.GetAt<T1>(0), arguments.GetAt<T2>(1), arguments.GetAt<T3>(2), arguments.GetAt<T4>(3)));

    internal static LispPrimitive Define<T1, T2, T3, T4, T5, TResult> (string name, Func<LispEnvironment, T1, T2, T3, T4, T5, TResult> body)
        where T1 : LispValue where T2 : LispValue where T3 : LispValue where T4 : LispValue where T5 : LispValue where TResult : LispValue =>
        new (name, (environment, arguments) =>
            arguments.Count != 5
                ? throw new ArgumentCountException(5, arguments.Count, name)
                : body(environment, arguments.GetAt<T1>(0), arguments.GetAt<T2>(1), arguments.GetAt<T3>(2), arguments.GetAt<T4>(3), arguments.GetAt<T5>(4)));

    internal static LispPrimitive DefineVarArg<TResult> (string name, Func<LispEnvironment, LispSequential, TResult> body)
        where TResult : LispValue =>
        new (name, body);

    internal static LispPrimitive DefineVarArg<T1, TResult> (string name, Func<LispEnvironment, T1, LispSequential, TResult> body)
        where T1: LispValue where TResult : LispValue =>
        new(name, (environment, arguments) =>
            arguments.Count < 1
                ? throw new ArgumentCountException(1, arguments.Count, name)
                : body(environment, arguments.GetAt<T1>(0), new LispList(arguments.Values.Skip(1))));
    internal static LispPrimitive DefineVarArg<T1, T2, TResult> (string name, Func<LispEnvironment, T1, T2, LispSequential, TResult> body)
        where T1: LispValue where T2: LispValue where TResult : LispValue =>
        new(name, (environment, arguments) =>
            arguments.Count < 2
                ? throw new ArgumentCountException(2, arguments.Count, name)
                : body(environment, arguments.GetAt<T1>(0), arguments.GetAt<T2>(1), new LispList(arguments.Values.Skip(2))));
}
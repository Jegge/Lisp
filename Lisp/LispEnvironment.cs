using System.Diagnostics;
using System.Reflection;
using Lisp.Types;

namespace Lisp;

[Flags]
public enum LispAccess
{
    None = 0,
    TerminateProcess = 1,
    ReadFiles = 2,
    WriteFiles = 4,
    ExecuteSystem = 8,
    All = int.MaxValue
}

public sealed class LispEnvironment
{
    private readonly LispEnvironment? _parent;
    private readonly Dictionary<string, LispValue> _values;

    public TextWriter? Output { get; init; }

    public LispEnvironment (LispAccess access = LispAccess.None) : this([], access) { }

    public LispEnvironment (string[] argc, LispAccess access = LispAccess.None)
    {
        _parent = null;
        _values = new Dictionary<string, LispValue>
        {
            // Special variables
            ["argv"] = new LispList(argc.Select(LispValue (a) => new LispString(a))),
            ["core-version"] = new LispString(Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "?.?.?.?"),
            ["DEBUG-EVAL"] = LispValue.Nil,
            ["access"] = new LispList(Enum.GetValues<LispAccess>()
                                            .Where(f => f > 0 && (int)f < int.MaxValue && access.HasFlag(f))
                                            .Select(LispValue (f) => new LispKeyword(f))
            ),

            // Special system functions
            ["env"] = LispPrimitive.Define("env", environment =>
            {
                var result = new Dictionary<LispValue, LispValue>();
                var current = environment;
                while (current is not null)
                {
                    foreach (var entry in current._values)
                        if (!result.ContainsKey(new LispSymbol(entry.Key)))
                            result[new LispSymbol(entry.Key)] = entry.Value;
                    current = current._parent;
                }
                return new LispHashMap(result);
            }),
            ["type-of"] = LispPrimitive.Define("type-of", (LispEnvironment _, LispValue value) => new LispSymbol(value.GetLispType())),
            ["nil?"] = LispPrimitive.Define("nil?", (LispEnvironment _, LispValue value) => new LispBool(value is LispNil)),
            ["lambda?"] = LispPrimitive.Define("lambda?", (LispEnvironment _, LispValue value) => new LispBool(value is LispApplicable)),
            ["macro?"] = LispPrimitive.Define("macro?", (LispEnvironment _, LispValue value) => new LispBool(value is LispLambda { IsMacro: true })),
            ["throw"] = LispPrimitive.Define("throw", LispValue (LispEnvironment _, LispString message) => throw new RuntimeException(message.Value)),

            // Arithmetic operators
            ["+"] = LispPrimitive.Define("+", (lhs, rhs) => lhs + rhs),
            ["-"] = LispPrimitive.Define("-", (lhs, rhs) => lhs - rhs),
            ["*"] = LispPrimitive.Define("*", (lhs, rhs) => lhs * rhs),
            ["/"] = LispPrimitive.Define("/", (lhs, rhs) => lhs / rhs),
            ["mod"] = LispPrimitive.Define("mod", (lhs, rhs) => lhs % rhs),

            // Boolean operators
            ["&&"] = LispPrimitive.Define("&&", (lhs, rhs) => lhs && rhs),
            ["||"] = LispPrimitive.Define("||", (lhs, rhs) => lhs || rhs),

            // Equality & numerical comparison
            ["="] = LispPrimitive.Define("=", (LispEnvironment _, LispValue lhs, LispValue rhs) => new LispBool(lhs.Equals(rhs))),
            ["<"] = LispPrimitive.Define("<", (lhs, rhs) => lhs < rhs),
            [">"] = LispPrimitive.Define(">", (lhs, rhs) => lhs > rhs),
            ["<="] = LispPrimitive.Define("<=", (lhs, rhs) => lhs <= rhs),
            [">="] = LispPrimitive.Define(">=", (lhs, rhs) => lhs >= rhs),

            // Conversions
            ["vec"] = LispPrimitive.Define("vec", (LispEnvironment _, LispSequential arg) => new LispVector(arg.Values)),
            ["seq"] = LispPrimitive.Define("seq", (LispEnvironment _, LispValue arg) => arg switch
            {
                LispList list => list,
                LispVector vector => new LispList(vector.Values),
                LispString str => new LispList(str.Value.ToCharArray().Select(LispValue (c) => new LispString(c.ToString()))),
                _ => LispValue.Nil
            }),

            // Sequence (aka List & Vector) functions
            ["cons"] = LispPrimitive.Define("cons", (LispEnvironment _, LispValue value, LispSequential seq) => new LispList(seq.Values.Prepend(value))),
            ["concat"] = LispPrimitive.DefineVarArg("concat", (_, seq) => new LispList(seq.As<LispSequential>().SelectMany(c => c.Values))),
            ["first"] = LispPrimitive.Define("first", (LispEnvironment _, LispSequential seq) => seq.Values.FirstOrDefault() ?? LispValue.Nil),
            ["rest"] = LispPrimitive.Define("rest", (LispEnvironment _, LispSequential seq) => new LispList(seq.Values.Skip(1))),
            ["nth"] = LispPrimitive.Define("nth", (LispEnvironment _, LispSequential seq, LispNumber index) => seq.Values[(int)index.Value]),
            ["list"] = LispPrimitive.DefineVarArg("list", (_, seq) => new LispList(seq.Values)),
            ["vector"] = LispPrimitive.DefineVarArg("vector", (_, seq) => new LispVector(seq.Values)),

            // Container (aka List, Vector, Hashmap) functions
            ["null?"] = LispPrimitive.Define("null?", (LispEnvironment _, LispContainer container) => new LispBool(container.Count == 0)),
            ["length"] = LispPrimitive.Define("length", (LispEnvironment _, LispContainer container) => new LispNumber(container.Count)),

            // Atom functions
            ["atom"] = LispPrimitive.Define("atom", (LispEnvironment _, LispValue value) => new LispAtom(value)),
            ["deref"] = LispPrimitive.Define("deref", (LispEnvironment _, LispAtom atom) => atom.Value),
            ["reset"] = LispPrimitive.Define("reset", (LispEnvironment _, LispAtom atom, LispValue value) => atom.Value = value),
            ["swap"] = LispPrimitive.DefineVarArg("swap", (LispEnvironment environment, LispAtom atom, LispApplicable applicable, LispSequential seq) =>
            {
                lock (atom)
                {
                    return atom.Value = new LispList(new List<LispValue> { applicable, atom.Value }.Concat(seq.Values)).Eval(environment);
                }
            }),

            // Hashmap functions
            ["hashmap"] = LispPrimitive.DefineVarArg("hashmap", (_, values) => new LispHashMap(values.TuplesOf<LispValue, LispValue>().ToDictionary())),
            ["contains?"] = LispPrimitive.Define("contains?", (LispEnvironment _, LispHashMap hashMap, LispValue key) => new LispBool(hashMap.Values.ContainsKey(key))),
            ["assoc"] = LispPrimitive.DefineVarArg("assoc", (LispEnvironment _, LispHashMap hashMap, LispSequential values) => hashMap.Assoc(values.TuplesOf<LispValue, LispValue>())),
            ["dissoc"] = LispPrimitive.DefineVarArg("dissoc", (LispEnvironment _, LispHashMap hashMap, LispSequential keys) => hashMap.Dissoc(keys.Values)),
            ["keys"] = LispPrimitive.Define("keys", (LispEnvironment _, LispHashMap hashMap) => new LispList(hashMap.Values.Keys)),
            ["values"] = LispPrimitive.Define("values", (LispEnvironment _, LispHashMap hashMap) => new LispList(hashMap.Values.Values)),
            ["get"] = LispPrimitive.Define("get", (LispEnvironment _, LispHashMap hashMap, LispValue key) => hashMap.Values.TryGetValue(key, out var value) ? value : LispValue.Nil),

            // Symbol functions
            ["symbol"] = LispPrimitive.Define("symbol", (LispEnvironment _, LispString name) => new LispSymbol(name.Value)),
            ["keyword"] = LispPrimitive.Define("keyword", (LispEnvironment _, LispString name) => new LispKeyword(name.Value)),

            // Core read / eval / print / apply functions
            ["read"] = LispPrimitive.Define("read", (LispEnvironment _, LispString content) => LispValue.Read(content.Value)),
            ["eval"] = LispPrimitive.Define("eval", (LispEnvironment _, LispValue value) => value.Eval(this)), // always evaluate in the root environment
            ["print"] = LispPrimitive.DefineVarArg("print", (_, seq) => new LispString(string.Join(' ', seq.Values.Select(a => a.Print(true))))),
            ["apply"] = LispPrimitive.DefineVarArg("apply", (environment, seq) =>
            {
                var args = seq.Values[1..^1].Concat(seq.GetAt<LispSequential>(seq.Count - 1).Values).ToArray();
                return seq.GetAt<LispValue>(0).Eval(environment) switch
                {
                    LispPrimitive primitive => primitive.Body(environment, new LispList(args)),
                    LispLambda { IsMacro: true } macro => macro.Body.Eval(new LispEnvironment(macro, args)),
                    LispLambda function => function.Body.Eval(new LispEnvironment(function, args)),
                    _ => throw new BadFormException(new LispList(seq))
                };
            }),

            // String function
            ["strcat"] = LispPrimitive.DefineVarArg("strcat", (_, seq) => 
                new LispString(string.Join(string.Empty, seq.Values.Select(a => a.Print(false))))),
            ["string<?"] = LispPrimitive.Define("string<?", (LispEnvironment _, LispString lhs, LispString rhs) =>
                new LispBool(string.Compare(lhs.Value, rhs.Value, StringComparison.InvariantCulture) < 0)),
            ["string>?"] = LispPrimitive.Define("string>?", (LispEnvironment _, LispString lhs, LispString rhs) =>
                new LispBool(string.Compare(lhs.Value, rhs.Value, StringComparison.InvariantCulture) > 0)),
            ["string<=?"] = LispPrimitive.Define("string<=?", (LispEnvironment _, LispString lhs, LispString rhs) =>
                new LispBool(string.Compare(lhs.Value, rhs.Value, StringComparison.InvariantCulture) <= 0)),
            ["string>=?"] = LispPrimitive.Define("string>=?", (LispEnvironment _, LispString lhs, LispString rhs) =>
                new LispBool(string.Compare(lhs.Value, rhs.Value, StringComparison.InvariantCulture) >= 0)),

            // IO port functions
            ["file-open-read"] = LispPrimitive.Define("file-open-read", (LispEnvironment _, LispString filepath) =>
                access.HasFlag(LispAccess.ReadFiles) ? new LispIoPort(filepath.Value, FileAccess.Read) : throw new AccessDeniedException(LispAccess.ReadFiles)),
            ["file-open-write"] = LispPrimitive.Define("file-open-write", (LispEnvironment _, LispString filepath) =>
                access.HasFlag(LispAccess.WriteFiles) ? new LispIoPort(filepath.Value, FileAccess.Write) : throw new AccessDeniedException(LispAccess.WriteFiles)),
            ["file-close"] = LispPrimitive.Define("file-close", (LispEnvironment _, LispIoPort port) => new LispBool(port.Close())),
            ["file-read"] =  LispPrimitive.Define("file-read", (LispEnvironment _, LispIoPort port) =>
                access.HasFlag(LispAccess.ReadFiles) ? (LispValue)(port.Read() is { } s ? new LispString(s) : LispValue.Nil) : throw new AccessDeniedException(LispAccess.ReadFiles)),
            ["file-write"] =  LispPrimitive.Define("file-write", (LispEnvironment _, LispIoPort port, LispString value) =>
                !access.HasFlag(LispAccess.WriteFiles) ? throw new AccessDeniedException(LispAccess.WriteFiles) : new LispBool(port.Write(value.Value))),
            ["input-file?"] = LispPrimitive.Define("input-file?", (LispEnvironment _, LispValue value) => new LispBool(value is LispIoPort { Access: FileAccess.Read })),
            ["output-file?"] = LispPrimitive.Define("output-file?", (LispEnvironment _, LispValue value) => new LispBool(value is LispIoPort { Access: FileAccess.Write })),

            // System functions
            ["prn"] = LispPrimitive.DefineVarArg("prn", (_, seq) =>
            {
                Output?.WriteLine(string.Join(" ", seq.Values.Select(a => a.Print(true))));
                return LispValue.Nil;
            }),
            ["println"] = LispPrimitive.DefineVarArg("prn", (_, seq) =>
            {
                Output?.WriteLine(string.Join(" ", seq.Values.Select(a => a.Print(false))));
                return LispValue.Nil;
            }),
            ["time-ms"] = LispPrimitive.Define("time-ms", _ => new LispNumber(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())),
            ["guid"] = LispPrimitive.Define("guid", _ => new LispString(Guid.NewGuid().ToString())),
            ["random"] = LispPrimitive.Define("random", (LispEnvironment _, LispNumber upperBound) => new LispNumber(Random.Shared.NextInt64((long)upperBound.Value))),
            ["system"] = LispPrimitive.DefineVarArg("system", (LispEnvironment _, LispString filepath, LispSequential arguments) =>
            {
                if (!access.HasFlag(LispAccess.ExecuteSystem))
                    throw new AccessDeniedException(LispAccess.ExecuteSystem);

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        WindowStyle = ProcessWindowStyle.Hidden,
                        FileName = filepath.Value,
                        Arguments = string.Join(" ", arguments.Values.Select(v => v.Print(false))),
                        RedirectStandardOutput = true
                    }
                };
                process.Start();
                process.WaitForExit();

                return new LispList(new LispNumber(process.ExitCode), new LispString(process.StandardOutput.ReadToEnd()));
            }),
            ["exit"] = LispPrimitive.Define("exit", LispNil (LispEnvironment _, LispNumber code) =>
            {
                if (access.HasFlag(LispAccess.TerminateProcess))
                    Environment.Exit((int)code.Value);
                throw new AccessDeniedException(LispAccess.TerminateProcess);
            })
        };

        using var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("Lisp.StdLib.lisp")!;
        using var reader = new StreamReader(resource);
        ReadEvaluatePrint($"(do {reader.ReadToEnd()}\nnil)\n");
    }

    internal LispEnvironment (LispLambda lambda, LispValue[] arguments)
    {
        _parent = lambda.Environment;
        _values = [];

        Output = lambda.Environment.Output;

        if (lambda.Arguments.Length != arguments.Length && lambda.VarArg is null)
            throw new ArgumentCountException(lambda.Arguments.Length, arguments.Length);

        foreach (var (name, value) in lambda.Arguments.Zip(arguments))
            _values[name.Value] = value;

        if (lambda.VarArg is not null)
            _values[lambda.VarArg.Value] = new LispList(arguments.Skip(lambda.Arguments.Length));
    }
    internal LispEnvironment (LispEnvironment parent, IEnumerable<(LispSymbol Symbol, LispValue Value)> bindings)
    {
        _parent = parent;
        _values = [];

        Output = parent.Output;

        foreach (var (symbol, value) in bindings)
            _values[symbol.Value] = value.Eval(this);
    }

    internal LispValue this [LispSymbol symbol]
    {
        get => _values.TryGetValue(symbol.Value, out var result)
            ? result
            : _parent?[symbol] ??  throw new SymbolNotFoundException(symbol);
        set => _values[symbol.Value] = value;
    }

    internal bool ContainsSymbol (LispSymbol symbol) =>
        _values.ContainsKey(symbol.Value) || (_parent?.ContainsSymbol(symbol) ?? false);

    /// <summary>
    /// Read the content of the current environment
    /// </summary>
    /// <param name="symbol">The name of the symbol</param>
    public LispValue? this [string symbol] =>
        _values.TryGetValue(symbol, out var result) ? result : _parent?[symbol];

    /// <summary>
    /// Registers a primitive with a given name in this environment, and overwrites any previous definition.
    /// </summary>
    /// <param name="name">The name of the primitive.</param>
    /// <param name="function">The function to be executed.</param>
    public void Register (string name, Func<LispEnvironment, LispValue[], LispValue> function) =>
        _values[name] = LispPrimitive.DefineVarArg(name, (environment, arguments) => function(environment, arguments.Values));

    /// <summary>
    /// Registers a primitive with a given name in this environment, and overwrites any previous definition.
    /// </summary>
    /// <param name="name">The name of the primitive.</param>
    /// <param name="function">The function to be executed.</param>
    public void Register (string name, Func<LispEnvironment, LispValue> function) =>
        _values[name] = LispPrimitive.Define(name, function);

    /// <summary>
    /// Registers a primitive with a given name in this environment, and overwrites any previous definition.
    /// </summary>
    /// <param name="name">The name of the primitive.</param>
    /// <param name="function">The function to be executed.</param>
    public void Register<T1> (string name, Func<LispEnvironment, T1, LispValue> function)
        where T1 : LispValue =>
        _values[name] = LispPrimitive.Define(name, function);

    /// <summary>
    /// Registers a primitive with a given name in this environment, and overwrites any previous definition.
    /// </summary>
    /// <param name="name">The name of the primitive.</param>
    /// <param name="function">The function to be executed.</param>
    public void Register<T1, T2> (string name, Func<LispEnvironment, T1, T2, LispValue> function)
        where T1 : LispValue where T2 : LispValue =>
        _values[name] = LispPrimitive.Define(name, function);

    /// <summary>
    /// Registers a primitive with a given name in this environment, and overwrites any previous definition.
    /// </summary>
    /// <param name="name">The name of the primitive.</param>
    /// <param name="function">The function to be executed.</param>
    public void Register<T1, T2, T3> (string name, Func<LispEnvironment, T1, T2, T3, LispValue> function)
        where T1 : LispValue where T2 : LispValue where T3 : LispValue =>
        _values[name] = LispPrimitive.Define(name, function);

    /// <summary>
    /// Registers a primitive with a given name in this environment, and overwrites any previous definition.
    /// </summary>
    /// <param name="name">The name of the primitive.</param>
    /// <param name="function">The function to be executed.</param>
    public void Register<T1, T2, T3, T4> (string name, Func<LispEnvironment, T1, T2, T3, T4, LispValue> function)
        where T1 : LispValue where T2 : LispValue where T3 : LispValue where T4 : LispValue =>
        _values[name] = LispPrimitive.Define(name, function);

    /// <summary>
    /// Registers a primitive with a given name in this environment, and overwrites any previous definition.
    /// </summary>
    /// <param name="name">The name of the primitive.</param>
    /// <param name="function">The function to be executed.</param>
    public void Register<T1, T2, T3, T4, T5> (string name, Func<LispEnvironment, T1, T2, T3, T4, T5, LispValue> function)
        where T1 : LispValue where T2 : LispValue where T3 : LispValue where T4 : LispValue where T5 : LispValue =>
        _values[name] = LispPrimitive.Define(name, function);

    /// <summary>
    /// Registers a primitive with a given name in this environment, and overwrites any previous definition.
    /// This primitive always returns 'nil'.
    /// </summary>
    /// <param name="name">The name of the primitive.</param>
    /// <param name="action">The action to be executed.</param>
    public void Register (string name, Action<LispEnvironment, LispValue[]> action) =>
        _values[name] = LispPrimitive.DefineVarArg(name, (environment, arguments)=>
        {
            action(environment, arguments.Values);
            return LispValue.Nil;
        });

    /// <summary>
    /// Registers a primitive with a given name in this environment, and overwrites any previous definition.
    /// This primitive always returns 'nil'.
    /// </summary>
    /// <param name="name">The name of the primitive.</param>
    /// <param name="action">The action to be executed.</param>
    public void Register (string name, Action<LispEnvironment> action) =>
        _values[name] = LispPrimitive.Define(name, environment =>
        {
            action(environment);
            return LispValue.Nil;
        });

    /// <summary>
    /// Registers a primitive with a given name in this environment, and overwrites any previous definition.
    /// This primitive always returns 'nil'.
    /// </summary>
    /// <param name="name">The name of the primitive.</param>
    /// <param name="action">The action to be executed.</param>
    public void Register<T1> (string name, Action<LispEnvironment, T1> action)
        where T1 : LispValue =>
        _values[name] = LispPrimitive.Define(name, (LispEnvironment environment, T1 arg1) =>
        {
            action(environment, arg1);
            return LispValue.Nil;
        });

    /// <summary>
    /// Registers a primitive with a given name in this environment, and overwrites any previous definition.
    /// This primitive always returns 'nil'.
    /// </summary>
    /// <param name="name">The name of the primitive.</param>
    /// <param name="action">The action to be executed.</param>
    public void Register<T1, T2> (string name, Action<LispEnvironment, T1, T2> action)
        where T1 : LispValue where T2 : LispValue =>
        _values[name] = LispPrimitive.Define(name, (LispEnvironment environment, T1 arg1, T2 arg2) =>
        {
            action(environment, arg1, arg2);
            return LispValue.Nil;
        });

    /// <summary>
    /// Registers a primitive with a given name in this environment, and overwrites any previous definition.
    /// This primitive always returns 'nil'.
    /// </summary>
    /// <param name="name">The name of the primitive.</param>
    /// <param name="action">The action to be executed.</param>
    public void Register<T1, T2, T3> (string name, Action<LispEnvironment, T1, T2, T3> action)
        where T1 : LispValue where T2 : LispValue where T3 : LispValue =>
        _values[name] = LispPrimitive.Define(name, (LispEnvironment environment, T1 arg1, T2 arg2, T3 arg3) =>
        {
            action(environment, arg1, arg2, arg3);
            return LispValue.Nil;
        });

    /// <summary>
    /// Registers a primitive with a given name in this environment, and overwrites any previous definition.
    /// This primitive always returns 'nil'.
    /// </summary>
    /// <param name="name">The name of the primitive.</param>
    /// <param name="action">The action to be executed.</param>
    public void Register<T1, T2, T3, T4> (string name, Action<LispEnvironment, T1, T2, T3, T4> action)
        where T1 : LispValue where T2 : LispValue where T3 : LispValue where T4 : LispValue =>
        _values[name] = LispPrimitive.Define(name, (LispEnvironment environment, T1 arg1, T2 arg2, T3 arg3, T4 arg4) =>
        {
            action(environment, arg1, arg2, arg3, arg4);
            return LispValue.Nil;
        });

    /// <summary>
    /// Registers a primitive with a given name in this environment, and overwrites any previous definition.
    /// This primitive always returns 'nil'.
    /// </summary>
    /// <param name="name">The name of the primitive.</param>
    /// <param name="action">The action to be executed.</param>
    public void Register<T1, T2, T3, T4, T5> (string name, Action<LispEnvironment, T1, T2, T3, T4, T5> action)
        where T1 : LispValue where T2 : LispValue where T3 : LispValue where T4 : LispValue where T5 : LispValue =>
        _values[name] = LispPrimitive.Define(name, (LispEnvironment environment, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) =>
        {
            action(environment, arg1, arg2, arg3, arg4, arg5);
            return LispValue.Nil;
        });

    /// <summary>
    /// Registers a constant string value with a given name and overwrites any previous definition.
    /// </summary>
    /// <param name="name">The name of the value.</param>
    /// <param name="value">The value.</param>
    public void Register (string name, string value) => _values[name] = new LispString(value);

    /// <summary>
    /// Registers a constant decimal value with a given name and overwrites any previous definition.
    /// </summary>
    /// <param name="name">The name of the value.</param>
    /// <param name="value">The value.</param>
    public void Register (string name, decimal value) => _values[name] = new LispNumber(value);

    /// <summary>
    /// Registers a constant bool value with a given name and overwrites any previous definition.
    /// </summary>
    /// <param name="name">The name of the value.</param>
    /// <param name="value">The value.</param>
    public void Register(string name, bool value) => _values[name] = new LispBool(value);

    /// <summary>
    /// Loads a lisp file and executes it in the root environment.
    /// </summary>
    /// <param name="filepath">The filepath of the file to load.</param>
    /// <exception cref="LispException"/>
    /// <exception cref="FileNotFoundException"/>
    /// <returns>The evaluation result of the file's content.</returns>
    public string LoadFile (string filepath) => ReadEvaluatePrint($"(load-file \"{LispString.Escape(filepath)}\")");

    /// <summary>
    /// Evaluates a string containing lisp and returns the evaluation result.
    /// </summary>
    /// <param name="input">The string to be executed.</param>
    /// <exception cref="LispException"/>
    /// <returns>The evaluation result.</returns>
    public string ReadEvaluatePrint(string input) => LispValue.Read(input).Eval(this).Print(true);
}

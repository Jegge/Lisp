using System.Diagnostics;
using Lisp.Parser;

namespace Lisp.Types;

public abstract class LispValue
{
    internal static string GetLispType (Type type) => type.Name.Replace("Lisp", string.Empty).ToLowerInvariant();
    internal string GetLispType () => GetLispType(GetType());
    internal static string GetLispType<T> () where T : LispValue => GetLispType(typeof(T));

    public static LispValue Read (string input) => new Queue<LispToken>(input.Tokenize()).Parse();
    // This method is not OO, to enable tail recursion
    public LispValue Eval (LispEnvironment environment)
    {
        var value = this;
        while (true)
        {
            if (environment[LispSymbol.DebugEval] is not (LispNil or LispBool { Value: false }))
                Trace.WriteLine($"EVAL: {value.Print(false)}");

            switch (value)
            {
                case LispSymbol symbol:
                    return environment[symbol];

                case LispVector vector:
                    return new LispVector(vector.Values.Select(v => v.Eval(environment)).ToList());

                case LispHashMap hashmap:
                    return new LispHashMap(hashmap.Values.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Eval(environment)));

                case LispList { Values: [] } list:
                    return list;

                // Special form: bound?
                case LispList { Values: [ LispSymbol { Value: LispSymbol.Token.BoundP }, LispSymbol { Value: LispSymbol.Token.BoundP } ]}:
                case LispList { Values: [ LispSymbol { Value: LispSymbol.Token.BoundP }, LispSymbol { Value: LispSymbol.Token.Try } ]}:
                case LispList { Values: [ LispSymbol { Value: LispSymbol.Token.BoundP }, LispSymbol { Value: LispSymbol.Token.Catch } ]}:
                case LispList { Values: [ LispSymbol { Value: LispSymbol.Token.BoundP }, LispSymbol { Value: LispSymbol.Token.Quote } ]}:
                case LispList { Values: [ LispSymbol { Value: LispSymbol.Token.BoundP }, LispSymbol { Value: LispSymbol.Token.Quasiquote } ]}:
                case LispList { Values: [ LispSymbol { Value: LispSymbol.Token.BoundP }, LispSymbol { Value: LispSymbol.Token.SpliceUnquote } ]}:
                case LispList { Values: [ LispSymbol { Value: LispSymbol.Token.BoundP }, LispSymbol { Value: LispSymbol.Token.Unquote } ]}:
                case LispList { Values: [ LispSymbol { Value: LispSymbol.Token.BoundP }, LispSymbol { Value: LispSymbol.Token.Do } ]}:
                case LispList { Values: [ LispSymbol { Value: LispSymbol.Token.BoundP }, LispSymbol { Value: LispSymbol.Token.Let } ]}:
                case LispList { Values: [ LispSymbol { Value: LispSymbol.Token.BoundP }, LispSymbol { Value: LispSymbol.Token.If } ]}:
                case LispList { Values: [ LispSymbol { Value: LispSymbol.Token.BoundP }, LispSymbol { Value: LispSymbol.Token.Define } ]}:
                case LispList { Values: [ LispSymbol { Value: LispSymbol.Token.BoundP }, LispSymbol { Value: LispSymbol.Token.DefineMacro } ]}:
                case LispList { Values: [ LispSymbol { Value: LispSymbol.Token.BoundP }, LispSymbol { Value: LispLambda.Token } ]}:
                    return new LispBool(true);

                case LispList { Values: [ LispSymbol { Value: LispSymbol.Token.BoundP }, LispSymbol symbol ]}:
                    return new LispBool(environment.ContainsSymbol(symbol));

                case LispList { Values: [ LispSymbol { Value: LispSymbol.Token.BoundP }, .. ]} list:
                    throw new BadFormException(list);

                // Special form: try / catch
                case LispList { Values: [ LispSymbol { Value: LispSymbol.Token.Try }, { } tryValue, LispList { Values: [LispSymbol { Value: LispSymbol.Token.Catch }, LispSymbol catchBinding, { } catchValue] } ] }:
                    try
                    {
                        return tryValue.Eval(environment);
                    }
                    catch (Exception exception)
                    {
                        environment = new LispEnvironment(environment,  [(catchBinding, new LispString(exception.Message))]);
                        value = catchValue;
                        continue;
                    }

                case LispList { Values: [LispSymbol { Value: LispSymbol.Token.Try }, ..] } list:
                    throw new BadFormException(list);

                // Special form: quote
                case LispList { Values: [LispSymbol { Value: LispSymbol.Token.Quote }, { } quoted] }:
                    return quoted;

                case LispList { Values: [LispSymbol { Value: LispSymbol.Token.Quote }, ..] } list:
                    throw new BadFormException(list);

                // Special form: quasiquote
                case LispList { Values: [LispSymbol { Value: LispSymbol.Token.Quasiquote }, { } quasiQuoted] }:
                    value = quasiQuoted.QuasiQuoteUnquoted();
                    continue;

                case LispList { Values: [LispSymbol { Value: LispSymbol.Token.Quasiquote }, ..] } list:
                    throw new BadFormException(list);

                // Special form: do
                case LispList { Values: [LispSymbol { Value: LispSymbol.Token.Do }, not null, ..] } list:
                {
                    foreach (var item in list.Values[1..^1])
                        item.Eval(environment);
                    value = list.Values.Last();
                    continue;
                }
                case LispList { Values: [LispSymbol { Value: LispSymbol.Token.Do }, ..] } list:
                    throw new BadFormException(list);

                // Special form: let
                case LispList { Values: [LispSymbol { Value: LispSymbol.Token.Let }, LispSequential letListBindings, { } letListValue] }:
                    environment = new LispEnvironment(environment,
                    letListBindings.TuplesOf<LispSymbol, LispValue>().ToArray());
                    value = letListValue;
                    continue;

                case LispList { Values: [LispSymbol { Value: LispSymbol.Token.Let }, ..] } list:
                    throw new BadFormException(list);

                // Special form: if
                case LispList { Values: [LispSymbol { Value: LispSymbol.Token.If }, { } condition, { } consequence] }:
                    value = condition.Eval(environment) is LispNil or LispBool { Value: false } ? new LispNil() : consequence;
                    continue;

                case LispList { Values: [ LispSymbol { Value: LispSymbol.Token.If }, { } condition, { } consequence, { } alternative ] }:
                    value = condition.Eval(environment) is LispNil or LispBool { Value: false } ? alternative : consequence;
                    continue;

                case LispList { Values: [LispSymbol { Value: LispSymbol.Token.If }, ..] } list:
                    throw new BadFormException(list);

                // Special form: define
                case LispList { Values: [LispSymbol { Value: LispSymbol.Token.Define }, LispSymbol binding, { } definition] }:
                {
                    var result = definition.Eval(environment);
                    environment[binding] = result;
                    return result;
                }
                case LispList { Values: [ LispSymbol { Value: LispSymbol.Token.Define }, LispList { Values: [LispSymbol binding, ..] signature }, { } body ] }:
                {
                    var result = new LispLambda(signature[1..].Cast<LispSymbol>(), null, body, environment);
                    environment[binding] = result;
                    return result;
                }
                case LispList { Values: [LispSymbol { Value: LispSymbol.Token.Define }, LispDotList { Head: [LispSymbol binding, ..] signature, Tail: LispSymbol varArg }, { } body ] }:
                {
                    var result = new LispLambda(signature[1..].Cast<LispSymbol>(), varArg, body, environment);
                    environment[binding] = result;
                    return result;
                }

                case LispList { Values: [LispSymbol { Value: LispSymbol.Token.Define }, ..] } list:
                    throw new BadFormException(list);

                // Special form: define-macro
                case LispList { Values: [LispSymbol { Value: LispSymbol.Token.DefineMacro }, LispSymbol binding, { } body] } list:
                {
                    if (body.Eval(environment) is not LispLambda macro)
                        throw new BadFormException(list);
                    macro.IsMacro = true;
                    environment[binding] = macro;
                    return macro;
                }

                case LispList { Values: [LispSymbol { Value: LispSymbol.Token.DefineMacro }, ..] } list:
                    throw new BadFormException(list);

                // Special form lambda
                case LispList { Values: [LispSymbol { Value: LispLambda.Token }, LispSymbol varArg, { } body] }:
                    return new LispLambda([], varArg, body, environment);

                case LispList { Values: [LispSymbol { Value: LispLambda.Token }, LispDotList { Head: var bindings, Tail: LispSymbol varArg }, { } body] }:
                    return new LispLambda(bindings.Cast<LispSymbol>(), varArg, body, environment);

                case LispList { Values: [LispSymbol { Value: LispLambda.Token }, LispSequential bindings, { } body] }:
                    return new LispLambda(bindings.Values.Cast<LispSymbol>(), null, body, environment);

                case LispList { Values: [LispSymbol { Value: LispLambda.Token }, ..] } list:
                    throw new BadFormException(list);

                // Application
                case LispList list:
                {
                    if (list.Values.First().Eval(environment) is not LispApplicable applicable)
                        throw new BadFormException(list);

                    switch (applicable)
                    {
                        case LispPrimitive primitive:
                            return primitive.Body(environment, new LispList(list.Values[1..].Select(v => v.Eval(environment))));

                        case LispLambda { IsMacro: false } function:
                            environment = new LispEnvironment(function, list.Values.Skip(1).Select(v => v.Eval(environment)).ToList());
                            value = function.Body;
                            continue;

                        case LispLambda { IsMacro: true } macro:
                            value = macro.Body.Eval(new LispEnvironment(macro, list.Values.Skip(1).ToList()));
                            continue;
                    }

                    break;
                }

                default:
                    return value;
            }
        }
    }
    public abstract string Print(bool readable);
    internal virtual LispValue QuasiQuoteUnquoted() => this;
}
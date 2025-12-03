using Lisp;
using LispRepl;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Lisp.Types;

Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = Encoding.UTF8;
Console.CancelKeyPress += (_, _) =>
{
    Console.ResetColor();
    Console.WriteLine();
    Console.WriteLine("Aborted.");
    Environment.Exit(0);
};

Trace.Listeners.Add(new ColorConsoleTraceListener(ConsoleColor.DarkGray));

var (executable, arguments) =  args switch
{
    ["--", ..] => (null, args[1..]),
    [var e, ..] => (e, args[1..]),
    _ => (null, args)
};

var version = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "?.?.?.?";

var environment = new LispEnvironment(arguments, LispAccess.All)
{
    Output = Console.Out
};

environment.Register("version", version);

environment.Register("read-json", (LispEnvironment env, LispString content) =>
{
    return ToLispValue(JsonDocument.Parse(content.Value).RootElement);

    LispValue ToLispValue (JsonElement element) =>
        element.ValueKind switch
        {
            JsonValueKind.Object => new LispHashMap(element.EnumerateObject()
                .ToDictionary(LispValue (property) => new LispSymbol(property.Name),
                    property => ToLispValue(property.Value))),
            JsonValueKind.Array => new LispVector(element.EnumerateArray().Select(ToLispValue)),
            JsonValueKind.String => (element.GetString() ?? string.Empty) switch
            {
                ['\'', '(', .., ')'] s => LispValue.Read(s).Eval(env),
                var s => new LispString(s)
            },
            JsonValueKind.Number => new LispNumber(element.GetDecimal()),
            JsonValueKind.True => new LispBool(true),
            JsonValueKind.False => new LispBool(false),
            JsonValueKind.Null or JsonValueKind.Undefined => LispValue.Nil,
            _ => throw new NotSupportedException($"Unsupported JsonValueKind {element.ValueKind}")
        };
});

environment.Register("write-json", (LispEnvironment _, LispValue content) =>
{
    var node = ToJsonNode(content);
    return node is null ? LispValue.Nil : new LispString(node.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

    JsonNode? ToJsonNode (LispValue value) =>
        value switch
        {
            LispAtom atom => ToJsonNode(atom.Value),
            LispHashMap hashmap => new JsonObject(hashmap.Values.Select(kvp =>
                new KeyValuePair<string, JsonNode?>(kvp.Key.Print(false), ToJsonNode(kvp.Value)))),
            LispKeyword keyword => keyword.Value,
            LispSequential sequential => new JsonArray(sequential.Values.Select(ToJsonNode).ToArray()),
            LispString s => s.Value,
            LispSymbol symbol => symbol.Value,
            LispNumber number => number.Value,
            LispBool b => b.Value,
            LispNil => null,
            _ => throw new NotSupportedException($"Unsupported LispValue {value.Print(true)}")
        };
});

if (executable is not null)
{
    try
    {
        Console.WriteLine(environment.LoadFile(executable));
    }
    catch (Exception exception)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Error.WriteLine(exception.Message);
        Console.ResetColor();
    }
}
else
{
    Console.WriteLine($"Welcome to lisp version {version}.");
    Console.WriteLine("Type (exit 0) to quit, Ctrl-C to abort.");
    Console.WriteLine();

    while (true)
    {
        Console.Write("user> ");
        var input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
            continue;

        try
        {
            Console.WriteLine(environment.ReadEvaluatePrint(input));
        }
        catch (Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Error.WriteLine("; " + exception.Message);
            Console.ResetColor();
        }
    }
}

return 0;

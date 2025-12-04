using System.Globalization;
using System.Text;
using Lisp.Types;

namespace Lisp.Parser;

public static class LispReader
{
    private static string? ReadUntil (this TextReader reader, Predicate<char> predicate, bool escaped = false)
    {
        var buffer = new StringBuilder();
        while (true) {
            var b = reader.Peek();
            if (b == -1)
                return null;

            if (escaped && b == '\\')
            {
                _ = reader.Read();
                switch (reader.Read())
                {
                    // this is a very special case: we append a '\n' at the end of our input string to make parsing easier
                    // So, if this \n occurs right after a stray '\\', we have to check for the end separately
                    case '\n':
                        if (reader.Peek() == -1)
                            return null;
                        break;
                    case 't':
                        buffer.Append('\t');
                        break;
                    case 'r':
                        buffer.Append('\r');
                        break;
                    case 'n':
                        buffer.Append('\n');
                        break;
                    case '\\':
                        buffer.Append('\\');
                        break;
                    case '"':
                        buffer.Append('"');
                        break;
                }
            }
            else
            {
                if (predicate((char)b))
                    break;
                buffer.Append((char)reader.Read());
            }
        }
        return buffer.ToString();
    }

    private static IEnumerable<LispToken> Tokenize (this string input)
    {
        var reader = new StringReader(input + '\n');

        var column = 0;
        var line = 0;

        while (true)
        {
            var b = reader.Read();
            if (b == -1)
                yield break;

            var c = (char)b;
            column++;

            if (" \t\r,".Contains(c))
                continue;

            if (c == '~' && reader.Peek() == '@')
            {
                yield return new LispToken(line, column, "~@");
                _ = reader.Read();
                column++;
                continue;
            }

            if (".'`~@()[]{}".Contains(c))
            {
                yield return new LispToken(line, column, c.ToString());
                continue;
            }

            switch (c)
            {
                case '\n':
                    column = 0;
                    line++;
                    continue;

                case ';':
                    _ = reader.ReadUntil(r => r == '\n');
                    column = 0;
                    line++;
                    continue;

                case '"':
                    if (reader.ReadUntil(r => r == '"', true) is not { } s)
                        throw new UnterminatedStringException(line, column);
                    yield return new LispToken(line, column, $"\"{s}\"");
                    _ = reader.Read();
                    column += s.Length + 1;
                    continue;

                default:
                    if (reader.ReadUntil(r => char.IsWhiteSpace(r) || ")}];".Contains(r)) is { } symbol)
                    {
                        yield return new LispToken(line, column, $"{(char)b}{symbol}");
                        column += symbol.Length;
                    }
                    continue;
            }
        }
    }

    private static IEnumerable<LispValue> ParseUntil (this Queue<LispToken> tokens, string stop, int line, int column)
    {
        while (tokens.TryPeek(out var element) && element.Value != stop)
            yield return tokens.Parse();

        if (!tokens.TryDequeue(out _))
            throw new UnbalancedParenthesisException(stop, line, column);
    }

    private static IEnumerable<(LispValue Key, LispValue Value)> ParsePairsUntil (this Queue<LispToken> tokens, string stop, int line, int column)
    {
        while (true)
        {
            if (!tokens.TryPeek(out var keyElement) || keyElement.Value == stop)
                break;

            var key = tokens.Parse();

            if (!tokens.TryPeek(out var valueElement) || valueElement.Value == stop)
                throw new UnexpectedEndOfInputException();

            var value = tokens.Parse();

            yield return (key, value);
        }

        if (!tokens.TryDequeue(out _))
            throw new UnbalancedParenthesisException(stop, line, column);
    }

    private static LispValue Parse (this Queue<LispToken> tokens)
    {
        if (!tokens.TryDequeue(out var token))
            throw new UnexpectedEndOfInputException();

        switch (token.Value)
        {
            case LispList.Token.End or LispVector.Token.End or LispHashMap.Token.End:
                throw new UnbalancedParenthesisException(token.Value, token.Line, token.Column);
            case LispList.Token.Begin:
                var list = tokens.ParseUntil(LispList.Token.End, token.Line, token.Column).ToArray();
                return (list.Count(c => c is LispSymbol { Value: LispDotList.Token.Dot }), list) switch
                {
                    (0, _) => new LispList(list),
                    (1, [.., not null, LispSymbol { Value: LispDotList.Token.Dot }, not null]) => new LispDotList(list[..^2], list.Last()),
                    _ => throw new UnbalancedParenthesisException(token.Value, token.Line, token.Column)
                };
            case LispVector.Token.Begin:
                return new LispVector(tokens.ParseUntil(LispVector.Token.End, token.Line, token.Column));
            case LispHashMap.Token.Begin:
                return new LispHashMap(tokens.ParsePairsUntil(LispHashMap.Token.End, token.Line, token.Column).ToDictionary());
            case "@":
                return new LispList(new LispSymbol(LispSymbol.Token.Deref), tokens.Parse());
            case "'":
                return new LispList(new LispSymbol(LispSymbol.Token.Quote), tokens.Parse());
            case "`":
                return new LispList(new LispSymbol(LispSymbol.Token.Quasiquote), tokens.Parse());
            case "~":
                return new LispList(new LispSymbol(LispSymbol.Token.Unquote), tokens.Parse());
            case "~@":
                return new LispList(new LispSymbol(LispSymbol.Token.SpliceUnquote), tokens.Parse());
            case LispNil.Token:
                return LispValue.Nil;
            case LispBool.Token.True:
                return new LispBool(true);
            case LispBool.Token.False:
                return new LispBool(false);
            case [LispKeyword.Token, ..]:
                return new LispKeyword(token.Value[1..]);
            case [LispString.Token.Delimiter, .., LispString.Token.Delimiter]:
                return new LispString(token.Value[1..^1]);
            default:
                if (decimal.TryParse(token.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var number))
                    return new LispNumber(number);
                return new LispSymbol(token.Value);
        }
    }

    public static LispValue Read (string input) => new Queue<LispToken>(input.Tokenize()).Parse();
}
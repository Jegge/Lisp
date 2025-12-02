using System.Globalization;
using System.Text;
using Lisp.Types;

namespace Lisp.Parser;

internal static class LispParser
{
    internal static IEnumerable<LispToken> Tokenize (this string input)
    {
        var isInComment = false;
        var isInString = false;
        var isAfterEscape = false;
        var line = 0;
        var column = -1;
        var buffer = new StringBuilder();

        var reader = new StringReader(input + '\n');

        var read = reader.Read();
        while (read != -1)
        {
            var c = (char)read;
            column++;

            switch (isInComment, isInString, isAfterEscape, c)
            {
                case (false, false, _, ';'):
                    if (buffer.Length > 0)
                        yield return new LispToken(line, column, buffer.ToString());
                    buffer.Clear();
                    isInComment = true;
                    break;

                case (false, _, _, '\r'):
                    break;

                case (true, _, _, '\n'):
                    buffer.Clear();
                    isInComment = false;
                    column = 0;
                    line++;
                    break;

                case (false, false, _, '\n'):
                    if (buffer.Length > 0)
                        yield return new LispToken(line, column, buffer.ToString());
                    buffer.Clear();
                    column = 0;
                    line++;
                    break;

                case (false, true, false, '\\'):
                    isAfterEscape = true;
                    break;

                case (false, false, _, '"'):
                    isInString = true;
                    break;

                case (false, true, false, '"'):
                    isInString = false;
                    yield return new LispToken(line, column, '"' + buffer.ToString() + '"');
                    buffer.Clear();
                    break;

                case (false, true, true, '"'):
                    isAfterEscape = false;
                    buffer.Append('"');
                    break;

                case (false, true, true, _):
                    isAfterEscape = false;
                    buffer.Append('\\');
                    buffer.Append(c);
                    break;

                case (false, false, _, ' '):
                case (false, false, _, '\t'):
                case (false, false, _, ','):
                    if (buffer.Length > 0)
                        yield return new LispToken(line, column, buffer.ToString());
                    buffer.Clear();
                    break;

                case (false, false, _, '~'):
                    if (buffer.Length > 0)
                        yield return new LispToken(line, column, buffer.ToString());
                    buffer.Clear();
                    buffer.Append('~');
                    if (reader.Peek() == '@')
                        buffer.Append((char)reader.Read());
                    yield return new LispToken(line, column, buffer.ToString());
                    buffer.Clear();
                    break;

                case (false, false, _, '@'):
                case (false, false, _, '\''):
                case (false, false, _, '`'):
                case (false, false, _, '('):
                case (false, false, _, ')'):
                case (false, false, _, '['):
                case (false, false, _, ']'):
                case (false, false, _, '{'):
                case (false, false, _, '}'):
                    if (buffer.Length > 0)
                        yield return new LispToken(line, column, buffer.ToString());
                    buffer.Clear();
                    yield return new LispToken(line, column, c.ToString());
                    break;

                default:
                    buffer.Append(c);
                    break;
            }

            read = reader.Read();
        }

        if (isInString)
            throw new UnterminatedStringException();
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
                throw new UnexpectedEndOfFileException();

            var value = tokens.Parse();

            yield return (key, value);
        }

        if (!tokens.TryDequeue(out _))
            throw new UnbalancedParenthesisException(stop, line, column);
    }

    internal static LispValue Parse (this Queue<LispToken> tokens)
    {
        if (!tokens.TryDequeue(out var token))
            throw new UnexpectedEndOfFileException();

        switch (token.Value)
        {
            case LispList.Token.End or LispVector.Token.End or LispHashMap.Token.End:
                throw new UnbalancedParenthesisException(token.Value, token.Line, token.Column);
            case LispList.Token.Begin:
                return tokens.ParseUntil(LispList.Token.End, token.Line, token.Column).ToArray() switch
                {
                    [.., LispSymbol { Value: LispDotList.Token.Dot }, { } tail] list => new LispDotList(list.SkipLast(2), tail),
                    var list => list.Contains(new LispSymbol(".")) ? throw new UnbalancedParenthesisException(token.Value, token.Line, token.Column) : new LispList(list)
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
                return new LispNil();
            case LispBool.Token.True:
                return new LispBool(true);
            case LispBool.Token.False:
                return new LispBool(false);
            case [LispKeyword.Token, ..]:
                return new LispKeyword(token.Value[1..]);
            case [LispString.Token.Delimiter, .., LispString.Token.Delimiter]:
                return new LispString(token.Value[1..^1].Unescaped());
            default:
                if (decimal.TryParse(token.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var number))
                    return new LispNumber(number);
                return new LispSymbol(token.Value);
        }
    }

    private static string Unescaped (this string text)
    {
        var buffer = new StringBuilder();
        var isAfterEscape = false;

        foreach (var c in text)
            switch (c, isAfterEscape)
            {
                case ('\\', false):
                    isAfterEscape = true;
                    break;

                case ('\\', true):
                    isAfterEscape = false;
                    buffer.Append('\\');
                    break;

                case ('t', true):
                    isAfterEscape = false;
                    buffer.Append('\t');
                    break;

                case ('r', true):
                    isAfterEscape = false;
                    buffer.Append('\r');
                    break;

                case ('n', true):
                    isAfterEscape = false;
                    buffer.Append('\n');
                    break;

                default:
                    buffer.Append(c);
                    break;
            }

        return buffer.ToString();
    }
}
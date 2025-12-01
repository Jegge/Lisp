using Lisp.Types;

namespace Lisp;

internal class LispException (string message) : Exception (message);

internal class UnterminatedStringException () : LispException ("Found unterminated string.");
internal class UnexpectedEndOfFileException () : LispException ("Unexpected end of file.");
internal class UnbalancedParenthesisException (string parenthesis, int line, int column) : LispException ($"Unbalanced parenthesis '{parenthesis}' at ({line}, {column}).");

internal class BadFormException (LispList list) : LispException ($"Bad form: {list.Print(false)}.");
internal class SymbolNotFoundException (LispSymbol symbol) : LispException ($"Symbol '{symbol.Print(true)}' not found.");
internal class ArgumentCountException (int expected, int actual, string? name = null) : LispException ($"Invalid argument count{(name is not null ? $" while calling '{name}'" : string.Empty)}: expected {expected}, but got {actual}.");
internal class TypeMismatchException<TExpected> (LispValue actual) : LispException ($"Type mismatch: expected {LispValue.GetLispType<TExpected>()}, but got {actual.GetLispType()}.") where TExpected : LispValue;
internal class RuntimeException (string message) : LispException($"Runtime error: {message}.");
internal class AccessDeniedException (LispAccess access) : LispException ($"Access {new LispKeyword(access).Print(true)} denied.");
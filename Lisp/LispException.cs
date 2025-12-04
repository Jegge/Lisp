using Lisp.Types;

namespace Lisp;

internal class LispException (string message)
    : Exception (message);

internal class UnterminatedStringException (int line, int column)
    : LispException ($"Found unterminated string at ({line}, {column}).");

internal class UnexpectedEndOfInputException ()
    : LispException ("Unexpected end of file.");

internal class UnbalancedParenthesisException (string parenthesis, int line, int column)
    : LispException ($"Unbalanced parenthesis '{parenthesis}' at ({line}, {column}).");

internal class BadFormException (LispList list)
    : LispException ($"Bad form: {list.Print(false)}.");

internal class SymbolNotFoundException (LispSymbol symbol)
    : LispException ($"Symbol '{symbol.Print(true)}' not found.");

internal class ArgumentCountException (int expected, int actual, string? name = null)
    : LispException ($"Invalid argument count{(name is not null ? $" while calling '{name}'" : string.Empty)}: expected {expected}, but got {actual}.");

internal class TypeMismatchException<T> (LispValue actual)
    : LispException ($"Type mismatch: expected {LispValue.GetLispType<T>()}, but got {actual.GetLispType()}.")
    where T : LispValue;

internal class TypeMismatchException<T1,T2> (LispValue actual)
    : LispException ($"Type mismatch: expected {LispValue.GetLispType<T1>()} or {LispValue.GetLispType<T2>()}, but got {actual.GetLispType()}.")
    where T1 : LispValue where T2 : LispValue;

internal class TypeMismatchException<T1,T2,T3> (LispValue actual)
    : LispException ($"Type mismatch: expected {LispValue.GetLispType<T1>()}, {LispValue.GetLispType<T2>()} or {LispValue.GetLispType<T3>()}, but got {actual.GetLispType()}.")
    where T1 : LispValue where T2 : LispValue where T3 : LispValue;

internal class RuntimeException (string message)
    : LispException($"Runtime error: {message}.");

internal class AccessDeniedException (LispAccess access)
    : LispException ($"Access {new LispKeyword(access).Print(true)} denied.");
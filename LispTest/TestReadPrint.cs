using Lisp;
using Lisp.Parser;
using Lisp.Types;

namespace LispTest;

[TestClass]
public sealed class TestReadPrint
{
    [TestMethod]
    public void Tokenize()
    {
        const string input = "abc 123.45\t\t(ab) \"test welt\" \ntrue   false nil ((blubb)) '(1 2)\n \"hallo \\\"welt\\\"!\" :keyword";
        var output = new[]
        {
            "abc", "123.45", "(", "ab", ")", "\"test welt\"",
            "true", "false", "nil", "(", "(", "blubb", ")", ")", "'", "(", "1", "2", ")",
            "\"hallo \"welt\"!\"", ":keyword"
        };
        var tokens = input.Tokenize().ToArray();

        Assert.AreEqual(output.Length, tokens.Length);

        foreach (var (expected, token) in output.Zip(tokens))
            Assert.AreEqual(expected, token.Value);
    }

    [TestMethod]
    [DataRow("(read \"(1 2 (3 4) nil)\")", "(1 2 (3 4) nil)")]
    [DataRow("(= nil (read \"nil\"))", "true")]
    [DataRow("(read \"(+ 2 3)\")", "(+ 2 3)")]
    [DataRow("(read \"\\\"\\n\\\"\")", "\"\\n\"")]
    [DataRow("(read \"7 ;; comment\")", "7")]
    public void ReadString (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("1", "1")]
    [DataRow("7", "7")]
    [DataRow(" 7", "7")]
    [DataRow("-123", "-123")]
    public void Numbers(string input, string expected)
    {
        Assert.AreEqual(expected, LispValue.Read(input).Print(true), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("+", "+")]
    [DataRow("-", "-")]
    [DataRow("*", "*")]
    [DataRow("/", "/")]
    [DataRow("abc", "abc")]
    [DataRow("abc:", "abc:")]
    [DataRow("   abc", "abc")]
    [DataRow("abc5", "abc5")]
    [DataRow("abc-def", "abc-def")]
    [DataRow("-def", "-def")]
    [DataRow("->>", "->>")]
    [DataRow("nil", "nil")]
    [DataRow("true", "true")]
    [DataRow("false", "false")]
    public void Symbols(string input, string expected)
    {
        Assert.AreEqual(expected, LispValue.Read(input).Print(true), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("\"abc\"", "\"abc\"")]
    [DataRow("\"abc  (with parens)\"", "\"abc  (with parens)\"")]
    [DataRow("\"abc\\\"def\"", "\"abc\\\"def\"")]
    [DataRow("\"abc\\\\def\"", "\"abc\\\\def\"")]
    [DataRow("\"abc\\tdef\"", "\"abc\\tdef\"")]
    [DataRow("\"\"", "\"\"")]
    [DataRow("\"abc\\ndef\"", "\"abc\\ndef\"")]
    public void Strings (string input, string expected)
    {
        Assert.AreEqual(expected, LispValue.Read(input).Print(true), "input:<{0}>", input);
    }

    [TestMethod]
    public void CommaAsWhitespace ()
    {
        Assert.AreEqual("(1 2 3)", LispValue.Read("(1 2, 3, ,,,,),,").Print(true));
    }

    [TestMethod]
    public void Deref()
    {
        Assert.AreEqual("(deref a)", LispValue.Read("@a").Print(true));
    }

    [TestMethod]
    [DataRow("'1", "(quote 1)")]
    [DataRow("'(1 2 3)", "(quote (1 2 3))")]
    [DataRow("`1", "(quasiquote 1)")]
    [DataRow("`(1 2 3)", "(quasiquote (1 2 3))")]
    [DataRow("`(a (b) c)", "(quasiquote (a (b) c))")]
    [DataRow("~1", "(unquote 1)")]
    [DataRow("~(1 2 3)", "(unquote (1 2 3))")]
    [DataRow("`(1 ~a 3)", "(quasiquote (1 (unquote a) 3))")]
    [DataRow("~@(1 2 3)", "(splice-unquote (1 2 3))")]
    public void Quoting(string input, string expected)
    {
        Assert.AreEqual(expected, LispValue.Read(input).Print(true), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow(":kw", ":kw")]
    [DataRow("(:kw1 :kw2 :kw3)", "(:kw1 :kw2 :kw3)")]
    public void Keywords(string input, string expected)
    {
        Assert.AreEqual(expected, LispValue.Read(input).Print(true), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("{}", "{}")]
    [DataRow("{ }", "{}")]
    [DataRow("{\"abc\" 1}", "{\"abc\" 1}")]
    [DataRow("{\"a\" { \"b\" {\"c\" 1}}}", "{\"a\" {\"b\" {\"c\" 1}}}")]
    [DataRow("{\"a\" { \"b\" {\"cde\" 3}}}", "{\"a\" {\"b\" {\"cde\" 3}}}")]
    [DataRow("{\"a1\" 1 \"a2\" 2 \"a3\" 3}", "{\"a1\" 1 \"a2\" 2 \"a3\" 3}")]
    [DataRow("{:a { :b {:c 3}}}", "{:a {:b {:c 3}}}")]
    [DataRow("{\"1\" 1}", "{\"1\" 1}")]
    [DataRow("({})", "({})")]
    public void Hashmaps(string input, string expected)
    {
        Assert.AreEqual(expected, LispValue.Read(input).Print(true), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(1 2")]
    [DataRow("[1 2")]
    [DataRow("{\"abc\" 2")]
    [DataRow(")")]
    [DataRow(") (+ 1 2)")]
    [DataRow("}")]
    [DataRow("} (+ 1 2)")]
    [DataRow("]")]
    [DataRow("] (+ 1 2)")]
    public void UnbalancedParenthesisException(string input)
    {
        Assert.ThrowsException<UnbalancedParenthesisException>(() => LispValue.Read(input).Print(true));
    }


    [TestMethod]
    [DataRow("\"abc")]
    [DataRow("\"")]
    [DataRow("\"\\\"")]
    [DataRow("\"\\\\\\\"")]
    [DataRow("(\"abc")]

    public void UnterminatedStringException(string input)
    {
        Assert.ThrowsException<UnterminatedStringException>(() => LispValue.Read(input).Print(true));
    }
}

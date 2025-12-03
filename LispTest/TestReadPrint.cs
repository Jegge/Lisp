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

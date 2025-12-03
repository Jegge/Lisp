using Lisp;
using Lisp.Parser;

namespace LispTest;

[TestClass]
public sealed class TestReadPrint
{
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
        Assert.AreEqual(expected, LispReader.Read(input).Print(true), "input:<{0}>", input);
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
        Assert.AreEqual(expected, LispReader.Read(input).Print(true), "input:<{0}>", input);
    }

    [TestMethod]
    public void CommaAsWhitespace ()
    {
        Assert.AreEqual("(1 2 3)", LispReader.Read("(1 2, 3, ,,,,),,").Print(true));
    }

    [TestMethod]
    [DataRow("\"abc")]
    [DataRow("\"")]
    [DataRow("\"\\\"")]
    [DataRow("\"\\\\\\\"")]
    [DataRow("(\"abc")]

    public void UnterminatedStringException(string input)
    {
        Assert.ThrowsException<UnterminatedStringException>(() => LispReader.Read(input).Print(true));
    }
}

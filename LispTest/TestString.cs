using Lisp;
using Lisp.Parser;
using Lisp.Types;

namespace LispTest;

[TestClass]
public sealed class TestString
{
    [TestMethod]
    [DataRow("\"abc\"", "\"abc\"")]
    [DataRow("\"abc  (with parens)\"", "\"abc  (with parens)\"")]
    [DataRow("\"abc\\\"def\"", "\"abc\\\"def\"")]
    [DataRow("\"abc\\\\def\"", "\"abc\\\\def\"")]
    [DataRow("\"abc\\tdef\"", "\"abc\\tdef\"")]
    [DataRow("\"\"", "\"\"")]
    [DataRow("\"abc\\ndef\"", "\"abc\\ndef\"")]
    public void ReadAndPrint (string input, string expected)
    {
        Assert.AreEqual(expected, LispReader.Read(input).Print(true), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(string>? \"abc\" \"acab\")", "false")]
    [DataRow("(string>? \"abc\" \"abc\")", "false")]
    [DataRow("(string>? \"acab\" \"abc\")", "true")]
    [DataRow("(string>=? \"abc\" \"acab\")", "false")]
    [DataRow("(string>=? \"acab\" \"abc\")", "true")]
    [DataRow("(string>=? \"abc\" \"abc\")", "true")]
    [DataRow("(string<? \"acab\" \"acab\")", "false")]
    [DataRow("(string<? \"abc\" \"acab\")", "true")]
    [DataRow("(string<? \"acab\" \"abc\")", "false")]
    [DataRow("(string<=? \"abc\" \"acab\")", "true")]
    [DataRow("(string<=? \"abc\" \"abc\")", "true")]
    [DataRow("(string<=? \"acab\" \"acab\")", "true")]
    public void Comparison (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(= \"\" \"\")", "true")]
    [DataRow("(= \"abc\" \"abc\")", "true")]
    [DataRow("(= \"abc\" \"\")", "false")]
    [DataRow("(= \"\" \"abc\")", "false")]
    [DataRow("(= \"abc\" \"def\")", "false")]
    [DataRow("(= \"abc\" \"ABC\")", "false")]
    [DataRow("(= (list) \"\")", "false")]
    [DataRow("(= \"\" (list))", "false")]
    public void Equality (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(type-of \"abc\")", typeof(LispString))]
    public void TypeOf (string input, Type expected)
    {
        Assert.AreEqual(LispValue.GetLispType(expected), new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(string? nil)", "false")]
    [DataRow("(string? \"\")", "true")]
    [DataRow("(string? \"test\")", "true")]
    [DataRow("(string? 2)", "false")]
    [DataRow("(string? (list))", "false")]
    [DataRow("(string? [])", "false")]
    [DataRow("(string? {})", "false")]
    [DataRow("(string? 'abc)", "false")]
    [DataRow("(string? :abc)", "false")]
    public void IsType (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
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

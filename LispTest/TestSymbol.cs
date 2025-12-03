using Lisp;
using Lisp.Parser;
using Lisp.Types;

namespace LispTest;

[TestClass]
public sealed class TestSymbol
{
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
    public void ReadAndPrint(string input, string expected)
    {
        Assert.AreEqual(expected, LispReader.Read(input).Print(true), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(symbol \"abc\")", "abc")]
    [DataRow("'abc", "abc")]
    public void Construction (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(= 'abc nil)", "false")]
    [DataRow("(= 'abc false)", "false")]
    [DataRow("(= 'abc true)", "false")]
    [DataRow("(= 'abc 0)", "false")]
    [DataRow("(= 'abc 1)", "false")]
    [DataRow("(= 'abc \"\")", "false")]
    [DataRow("(= 'abc ())", "false")]
    [DataRow("(= 'abc [])", "false")]
    [DataRow("(= 'abc {})", "false")]
    [DataRow("(= 'abc 'def)", "false")]
    [DataRow("(= 'abc 'abc)", "true")]
    public void Equality (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(type-of 'abc)", typeof(LispSymbol))]
    public void TypeOf (string input, Type expected)
    {
        Assert.AreEqual(LispValue.GetLispType(expected), new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(symbol? 'abc)", "true")]
    [DataRow("(symbol? \"abc\")", "false")]
    [DataRow("(symbol? :abc)", "false")]
    [DataRow("(symbol? (symbol \"abc\"))", "true")]
    public void IsType (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }
}

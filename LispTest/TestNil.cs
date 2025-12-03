using Lisp;
using Lisp.Parser;
using Lisp.Types;

namespace LispTest;

[TestClass]
public sealed class TestNil
{
    [TestMethod]
    [DataRow("nil", "nil")]
    public void ReadAndPrint (string input, string expected)
    {
        Assert.AreEqual(expected, LispReader.Read(input).Print(true), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(= nil nil)", "true")]
    [DataRow("(= nil :nil)", "false")]
    [DataRow("(= nil false)", "false")]
    [DataRow("(= nil true)", "false")]
    [DataRow("(= nil 0)", "false")]
    [DataRow("(= nil 1)", "false")]
    [DataRow("(= nil \"\")", "false")]
    [DataRow("(= nil '())", "false")]
    [DataRow("(= nil [])", "false")]
    [DataRow("(= nil {})", "false")]
    public void Equality (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(type-of nil)", typeof(LispNil))]
    public void TypeOf (string input, Type expected)
    {
        Assert.AreEqual(LispValue.GetLispType(expected), new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(nil? nil)", "true")]
    [DataRow("(nil? false)", "false")]
    [DataRow("(nil? true)", "false")]
    [DataRow("(nil? ())", "false")]
    [DataRow("(nil? 0)", "false")]
    public void IsType (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }
}

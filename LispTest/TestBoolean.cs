using Lisp;
using Lisp.Parser;
using Lisp.Types;

namespace LispTest;

public class TestBoolean
{
    [TestMethod]
    [DataRow("true", "true")]
    [DataRow("false", "false")]
    public void ReadAndPrint (string input, string expected)
    {
        Assert.AreEqual(expected, LispReader.Read(input).Print(true), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(= false nil)", "false")]
    [DataRow("(= false false)", "true")]
    [DataRow("(= false true)", "false")]
    [DataRow("(= false :false)", "false")]
    [DataRow("(= false 0)", "false")]
    [DataRow("(= false 1)", "false")]
    [DataRow("(= false \"\")", "false")]
    [DataRow("(= false ())", "false")]
    [DataRow("(= false [])", "false")]
    [DataRow("(= false {})", "false")]
    [DataRow("(= true nil)", "false")]
    [DataRow("(= true false)", "false")]
    [DataRow("(= true :true)", "false")]
    [DataRow("(= true true)", "true")]
    [DataRow("(= true 0)", "false")]
    [DataRow("(= true 1)", "false")]
    [DataRow("(= true \"\")", "false")]
    [DataRow("(= true ())", "false")]
    [DataRow("(= true [])", "false")]
    [DataRow("(= true {})", "false")]
    public void Equality (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(&& true true)", "true")]
    [DataRow("(&& true false)", "false")]
    [DataRow("(&& false true)", "false")]
    [DataRow("(&& false false)", "false")]
    [DataRow("(|| true true)", "true")]
    [DataRow("(|| true false)", "true")]
    [DataRow("(|| false true)", "true")]
    [DataRow("(|| false false)", "false")]
    public void Primitives (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(type-of true)", typeof(LispBool))]
    [DataRow("(type-of false)", typeof(LispBool))]
    public void TypeOf (string input, Type expected)
    {
        Assert.AreEqual(LispValue.GetLispType(expected), new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(true? nil)", "false")]
    [DataRow("(true? false)", "false")]
    [DataRow("(true? true)", "true")]
    [DataRow("(true? ())", "false")]
    [DataRow("(true? 0)", "false")]
    [DataRow("(true? true?)", "false")]
    [DataRow("(false? nil)", "false")]
    [DataRow("(false? false)", "true")]
    [DataRow("(false? true)", "false")]
    [DataRow("(false? 0)", "false")]
    [DataRow("(false? ())", "false")]
    [DataRow("(false? [])", "false")]
    [DataRow("(false? {})", "false")]
    [DataRow("(false? \"\")", "false")]
    public void IsType (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }
}
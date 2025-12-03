using Lisp;
using Lisp.Types;

namespace LispTest;

[TestClass]
public sealed class TestKeyword
{
    [TestMethod]
    [DataRow(":kw", ":kw")]
    [DataRow("(:kw1 :kw2 :kw3)", "(:kw1 :kw2 :kw3)")]
    public void ReadAndPrint (string input, string expected)
    {
        Assert.AreEqual(expected, LispValue.Read(input).Print(true), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(keyword \"abc\")", ":abc")]
    public void Construction (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(= :abc :abc)", "true")]
    [DataRow("(= :abc :def)", "false")]
    [DataRow("(= :abc \":abc\")", "false")]
    [DataRow("(= (list :abc) (list :abc))", "true")]
    public void Equality (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(type-of :abc)", typeof(LispKeyword))]
    public void TypeOf (string input, Type expected)
    {
        Assert.AreEqual(LispValue.GetLispType(expected), new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(keyword? :abc)", "true")]
    [DataRow("(keyword? 'abc)", "false")]
    [DataRow("(keyword? \"abc\")", "false")]
    [DataRow("(keyword? \"\")", "false")]
    [DataRow("(keyword? (keyword \"abc\"))", "true")]
    [DataRow("(keyword? nil)", "false")]
    [DataRow("(keyword? (list))", "false")]
    [DataRow("(keyword? [])", "false")]
    public void IsType (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }
}

using Lisp;
using Lisp.Parser;
using Lisp.Types;

namespace LispTest;

[TestClass]
public sealed class TestNumber
{
    [TestMethod]
    [DataRow("1", "1")]
    [DataRow("7", "7")]
    [DataRow(" 7", "7")]
    [DataRow("-123", "-123")]
    public void ReadAndPrint (string input, string expected)
    {
        Assert.AreEqual(expected, LispReader.Read(input).Print(true), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(+ 1 2)", "3")]
    [DataRow("(+ 5 (* 2 3))", "11")]
    [DataRow("(- (+ 5 (* 2 3)) 3)", "8")]
    [DataRow("(/ (- (+ 5 (* 2 3)) 3) 4)", "2")]
    [DataRow("(/ (- (+ 515 (* 87 311)) 302) 27)", "1010")]
    [DataRow("(* -3 6)", "-18")]
    [DataRow("(/ (- (+ 515 (* -87 311)) 296) 27)", "-994")]
    public void Primitives (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(> 2 1)", "true")]
    [DataRow("(> 1 1)", "false")]
    [DataRow("(> 1 2)", "false")]
    [DataRow("(>= 2 1)", "true")]
    [DataRow("(>= 1 1)", "true")]
    [DataRow("(>= 1 2)", "false")]
    [DataRow("(< 2 1)", "false")]
    [DataRow("(< 1 1)", "false")]
    [DataRow("(< 1 2)", "true")]
    [DataRow("(<= 2 1)", "false")]
    [DataRow("(<= 1 1)", "true")]
    [DataRow("(<= 1 2)", "true")]
    public void Comparison (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(= 2 1)", "false")]
    [DataRow("(= 1 1)", "true")]
    [DataRow("(= 1 2)", "false")]
    [DataRow("(= 1 (+ 1 1))", "false")]
    [DataRow("(= 2 (+ 1 1))", "true")]
    [DataRow("(= 23 nil)", "false")]
    [DataRow("(= 23 false)", "false")]
    [DataRow("(= 23 true)", "false")]
    [DataRow("(= 23 \"\")", "false")]
    [DataRow("(= 23 '())", "false")]
    [DataRow("(= 23 {})", "false")]
    [DataRow("(= 23 [])", "false")]
    [DataRow("(= 23 :abc)", "false")]
    [DataRow("(= 23 'abc)", "false")]
    public void Equality (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(type-of 23)", typeof(LispNumber))]
    [DataRow("(type-of 0)", typeof(LispNumber))]
    [DataRow("(type-of -42)", typeof(LispNumber))]
    public void TypeOf (string input, Type expected)
    {
        Assert.AreEqual(LispValue.GetLispType(expected), new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(number? nil)", "false")]
    [DataRow("(number? 2)", "true")]
    [DataRow("(number? 4.2)", "true")]
    [DataRow("(number? \"\")", "false")]
    [DataRow("(number? \"abc\")", "false")]
    [DataRow("(number? (list))", "false")]
    [DataRow("(number? [])", "false")]
    [DataRow("(number? :abc)", "false")]
    public void IsType (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }
}

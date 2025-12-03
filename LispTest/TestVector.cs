using Lisp;
using Lisp.Types;

namespace LispTest;

[TestClass]
public sealed class TestVector
{
    [TestMethod]
    [DataRow("[+ 1 2]", "[+ 1 2]")]
    [DataRow("[]", "[]")]
    [DataRow("[ ]", "[]")]
    [DataRow("[[3 4]]", "[[3 4]]")]
    [DataRow("[+ 1 [+ 2 3]]", "[+ 1 [+ 2 3]]")]
    [DataRow("  [ +   1   [+   2 3   ]   ]  ", "[+ 1 [+ 2 3]]")]
    [DataRow("([])", "([])")]
    public void ReadAndPrint (string input, string expected)
    {
        Assert.AreEqual(expected, LispValue.Read(input).Print(true), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("[1 2")]
    [DataRow("]")]
    [DataRow("] (+ 1 2)")]
    public void UnbalancedParenthesisException(string input)
    {
        Assert.ThrowsException<UnbalancedParenthesisException>(() => LispValue.Read(input).Print(true));
    }

    [TestMethod]
    [DataRow("[]", "[]")]
    [DataRow("[nil]", "[nil]")]
    [DataRow("[1 2 (+ 1 2)]", "[1 2 3]")]
    [DataRow("(vec '(1 2 3))", "[1 2 3]")]
    [DataRow("(vec (list))", "[]")]
    [DataRow("(vec []))", "[]")]
    [DataRow("(vec [1 2 3]))", "[1 2 3]")]
    public void Construction (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(null? [])", "true")]
    [DataRow("(null? [1])", "false")]
    public void IsNull (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(length [1 2 3])", "3")]
    [DataRow("(length [])", "0")]
    public void Length (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(sequential? '(1 2 3))", "true")]
    [DataRow("(list? [1 2 3])", "false")]
    [DataRow("(vector? [1 2 3])", "true")]
    [DataRow("(vector? [1 2 3])", "true")]
    public void IsType (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(= [] (list))", "true")]
    [DataRow("(= [7 8] [7 8])", "true")]
    [DataRow("(= [:abc] [:abc])", "true")]
    [DataRow("(= (list 1 2) [1 2])", "true")]
    [DataRow("(= (list 1) [])", "false")]
    [DataRow("(= [] [1])", "false")]
    [DataRow("(= 0 [])", "false")]
    [DataRow("(= [] 0)", "false")]
    [DataRow("(= [] \"\")", "false")]
    [DataRow("(= \"\" [])", "false")]
    [DataRow("(= [(list)] (list []))", "true")]
    [DataRow("(= [1 2 (list 3 4 [5 6])] (list 1 2 [3 4 (list 5 6)]))", "true")]
    public void Equality (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(print [1 2 \"abc\" \"\\\"\"] \"def\")", "\"[1 2 \\\"abc\\\" \\\"\\\\\\\"\\\"] \\\"def\\\"\"")]
    [DataRow("(print [])", "\"[]\"")]
    public void Print (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(type-of [])", typeof(LispVector))]
    [DataRow("(type-of [1 2 3])", typeof(LispVector))]
    public void TypeOf (string input, Type expected)
    {
        Assert.AreEqual(LispValue.GetLispType(expected), new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(cons 1 [])", "(1)")]
    [DataRow("(cons [1] [2 3])", "([1] 2 3)")]
    [DataRow("(cons 1 [2 3])", "(1 2 3)")]
    public void Cons (string input, string expected)
    {
        var sut = new LispEnvironment();
        sut.ReadEvaluatePrint("(define a (list 2 3))");
        Assert.AreEqual(expected, sut.ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(concat [1 2] (list 3 4) (list 5 6))", "(1 2 3 4 5 6)")]
    [DataRow("(concat [1 2])", "(1 2)")]
    public void Concat (string input, string expected)
    {
        var sut = new LispEnvironment();
        sut.ReadEvaluatePrint("(define a (list 2 3))");
        sut.ReadEvaluatePrint("(define b (list 4 5))");
        Assert.AreEqual(expected, sut.ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(first [])", "nil")]
    [DataRow("(first [6])", "6")]
    [DataRow("(first [6 7 8])", "6")]
    public void First (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(rest [])", "()")]
    [DataRow("(rest [6])", "()")]
    [DataRow("(rest [6 7 8])", "(7 8)")]
    public void Rest (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(nth [1] 0)", "1")]
    [DataRow("(nth [1 2] 1)", "2")]
    [DataRow("(nth [1 2 nil] 2)", "nil")]
    public void Nth (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(map (lambda (a) (* 2 a)) [1 2 3])", "(2 4 6)")]
    [DataRow("(map (lambda args (list? args)) [1 2])", "(true true)")]
    public void Map (string input, string expected)
    {
        var sut = new LispEnvironment();
        sut.ReadEvaluatePrint("(define numbers (list 1 2 3))");
        sut.ReadEvaluatePrint("(define double (lambda (x) (* 2 x)))");
        Assert.AreEqual(expected, sut.ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(filter even? numbers)", "(2 4 6 8 10)")]
    [DataRow("(filter (lambda (x) (= (mod x 2) 0)) [1 2 3 4 5 6 7 8 9 10])", "(2 4 6 8 10)")]
    public void Filter (string input, string expected)
    {
        var sut = new LispEnvironment();
        sut.ReadEvaluatePrint("(define numbers [1 2 3 4 5 6 7 8 9 10])");
        Assert.AreEqual(expected, sut.ReadEvaluatePrint(input), "input:<{0}>", input);
    }
}

using Lisp;

namespace TestLisp;

[TestClass]
public sealed class TestEnvironment
{


    [TestMethod]
    [DataRow(new[] { "(define x 3)", "x", "(define x 4)", "x", "(define y (+ 1 7))" }, new[] { "3", "3", "4", "4", "8", "8" })]
    [DataRow(new[] { "(define mynum 111)", "(define MYNUM 222)", "mynum", "MYNUM" }, new[] { "111", "222", "111", "222" })]
    public void Define(string[] input, string[] expected)
    {
        var sut = new LispEnvironment();
        foreach (var (i, e) in input.Zip(expected))
            Assert.AreEqual(e, sut.ReadEvaluatePrint(i), "input:<{0}>", i);
    }

    [TestMethod]
    public void DefineAbortsOnException()
    {
        var sut = new LispEnvironment();
        Assert.AreEqual("\"x\"", sut.ReadEvaluatePrint("(define x \"x\")"));
        Assert.ThrowsException<IndexOutOfRangeException>(() => sut.ReadEvaluatePrint("(define x (nth (list 1 2) 2))"));
        Assert.AreEqual("\"x\"", sut.ReadEvaluatePrint("x"));
    }

    [TestMethod]
    public void SymbolNotFoundException()
    {
        Assert.ThrowsException<SymbolNotFoundException>(() => new LispEnvironment().ReadEvaluatePrint("(abc 1 2 3)"));
    }

    [TestMethod]

    public void ErrorsAbortDefine ()
    {
        var sut = new LispEnvironment();
        Assert.AreEqual("123", sut.ReadEvaluatePrint("(define w 123)"));
        Assert.ThrowsException<SymbolNotFoundException>(() => sut.ReadEvaluatePrint("(define w (abc))"));
        Assert.AreEqual("123", sut.ReadEvaluatePrint("w"));
    }

    [TestMethod]
    [DataRow(new[] { "(define x 4)", "(let (z 9) z)", "(let (x 9) x)", "x" }, new[] { "4", "9", "9", "4" })]
    [DataRow(new[] { "(let (z (+ 2 3)) (+ 1 z))" }, new[] { "6" })]
    [DataRow(new[] { "(let (p (+ 2 3) q (+ 2 p)) (+ p q))" }, new[] { "12" })]
    [DataRow(new[] { "(define y (let (z 7) z))", "y" }, new[] { "7", "7" })]
    [DataRow(new[] { "(define a 4)", "(let (q 9) q)", "(let (q 9) a)", "(let (z 2) (let (q 9) a))" }, new[] { "4", "9", "4", "4" })]
    [DataRow(new[] { "(let (f (lambda () x) x 3) (f))" }, new[] { "3" })]
    [DataRow(new[] { "(let (cst (lambda (n) (if (= n 0) nil (cst (- n 1))))) (cst 1))" }, new[] { "nil" })]
    [DataRow(new[] { "(let (f (lambda (n) (if (= n 0) 0 (g (- n 1)))) g (lambda (n) (f n))) (f 2))" }, new[] { "0" })]
    public void Let(string[] input, string[] expected)
    {
        var sut = new LispEnvironment();
        foreach (var (i, e) in input.Zip(expected))
            Assert.AreEqual(e, sut.ReadEvaluatePrint(i), "input:<{0}>", i);
    }


    [TestMethod]
    public void CommandLineArguments ()
    {
        Assert.AreEqual("()", new LispEnvironment().ReadEvaluatePrint("argv"));
        Assert.AreEqual("(\"foo\")", new LispEnvironment(["foo"]).ReadEvaluatePrint("argv"));
        Assert.AreEqual("(\"foo\" \"bar\")", new LispEnvironment(["foo", "bar"]).ReadEvaluatePrint("argv"));
    }
}

using Lisp;

namespace LispTest;

[TestClass]
public sealed class TestSpecialForms
{
    [TestMethod]
    [DataRow("(if true 7 8)", "7")]
    [DataRow("(if false 7 8)", "8")]
    [DataRow("(if false 7 false)", "false")]
    [DataRow("(if true (+ 1 7) (+ 1 8))", "8")]
    [DataRow("(if false (+ 1 7) (+ 1 8))", "9")]
    [DataRow("(if nil 7 8)", "8")]
    [DataRow("(if 0 7 8)", "7")]
    [DataRow("(if (list) 7 8)", "7")]
    [DataRow("(if (list 1 2 3) 7 8)", "7")]
    [DataRow("(if false (+ 1 7))", "nil")]
    [DataRow("(if nil 8)", "nil")]
    [DataRow("(if nil 8 7)", "7")]
    [DataRow("(if true (+ 1 7))", "8")]
    [DataRow("(if [] 7 8)", "7")]
    public void If (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("((lambda (a b) (+ b a)) 3 4)", "7")]
    [DataRow("((lambda () 4) )", "4")]
    [DataRow("((lambda () ()) )", "()")]
    [DataRow("((lambda (f x) (f x)) (lambda (a) (+ 1 a)) 7)", "8")]
    [DataRow("(((lambda (a) (lambda (b) (+ a b))) 5) 7)", "12")]
    [DataRow("(define gen-plus5 (lambda () (lambda (b) (+ 5 b))))\n(define plus5 (gen-plus5))\n(plus5 7)", "12")]
    [DataRow("(define gen-plusX (lambda (x) (lambda (b) (+ x b))))\n(define plus7 (gen-plusX 7))\n(plus7 8)", "15")]
    [DataRow("(let [b 0 f (lambda [] b)] (let [b 1] (f)))", "0")]
    [DataRow("((let [b 0] (lambda [] b)))", "0")]
    [DataRow("((lambda [] 4) )", "4")]
    [DataRow("((lambda [f x] (f x)) (lambda [a] (+ 1 a)) 7)", "8")]
    public void Lambda (string input, string expected)
    {
        var sut = new LispEnvironment();
        var result = string.Empty;

        foreach (var i in input.Split('\n'))
            result = sut.ReadEvaluatePrint(i);

        Assert.AreEqual(expected, result, "input:<{0}>", input);
    }

    [TestMethod]
    //[DataRow("((lambda (& more) (length more)) 1 2 3)", "3")]
    [DataRow("((lambda more (length more)) 1 2 3)", "3")]
    [DataRow("((lambda more (list? more)) 1 2 3)", "true")]
    [DataRow("((lambda more (length more)) 1)", "1")]
    [DataRow("((lambda more (length more)) )", "0")]
    [DataRow("((lambda more (list? more)) )", "true")]
    [DataRow("((lambda (a . more) (length more)) 1 2 3)", "2")]
    [DataRow("((lambda (a . more) (length more)) 1)", "0")]
    [DataRow("((lambda (a . more) (list? more)) 1)", "true")]
    public void LambdaVariadic (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(do (+ 3 4))", "7")]
    [DataRow("(do (* 2 3) (+ 3 4))", "7")]
    [DataRow("(do (* 2 3) 7)", "7")]
    [DataRow("(do (define a 6) 7 (+ a 8))", "14")]
    [DataRow("(do (do 1 2))", "2")]
    public void Do (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

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
    public void Let (string[] input, string[] expected)
    {
        var sut = new LispEnvironment();
        foreach (var (i, e) in input.Zip(expected))
            Assert.AreEqual(e, sut.ReadEvaluatePrint(i), "input:<{0}>", i);
    }
}

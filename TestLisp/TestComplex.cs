namespace TestLisp;

using Lisp;

[TestClass]
public sealed class TestComplex
{
    [TestMethod]
    [DataRow(
        new[] { "(sumdown 1)", "(sumdown 2)", "(sumdown 6)" },
        new[] { "1", "3", "21" })
    ]
    public void SumDown (string[] input, string[] expected)
    {
        var sut = new LispEnvironment();
        sut.ReadEvaluatePrint("(define sumdown (lambda (N) (if (> N 0) (+ N (sumdown  (- N 1))) 0)))");

        foreach (var (i, e) in input.Zip(expected))
            Assert.AreEqual(e, sut.ReadEvaluatePrint(i), "input:<{0}>", i);
    }

    [TestMethod]
    [DataRow(
        new[] { "(fib 1)", "(fib 2)", "(fib 4)" },
        new[] { "1", "2", "5" })
    ]
    public void Fibonacci(string[] input, string[] expected)
    {
        var sut = new LispEnvironment();
        sut.ReadEvaluatePrint("(define fib (lambda (N) (if (= N 0) 1 (if (= N 1) 1 (+ (fib (- N 1)) (fib (- N 2)))))))");

        foreach (var (i, e) in input.Zip(expected))
            Assert.AreEqual(e, sut.ReadEvaluatePrint(i), "input:<{0}>", i);
    }

    [TestMethod]
    public void RecursiveTailCall ()
    {
        var sut = new LispEnvironment();
        Assert.AreEqual("(lambda (n acc) (if (= n 0) acc (sum2 (- n 1) (+ n acc))))", sut.ReadEvaluatePrint("(define sum2 (lambda (n acc) (if (= n 0) acc (sum2 (- n 1) (+ n acc)))))"));
        Assert.AreEqual("55", sut.ReadEvaluatePrint("(sum2 10 0)"));
        Assert.AreEqual("nil", sut.ReadEvaluatePrint("(define res2 nil)"));
        Assert.AreEqual("nil", sut.ReadEvaluatePrint("res2"));
        Assert.AreEqual("50005000", sut.ReadEvaluatePrint("(define res2 (sum2 10000 0))"));
        Assert.AreEqual("50005000", sut.ReadEvaluatePrint("res2"));
    }

    [TestMethod]
    public void MutuallyRecursiveTailCall()
    {
        var sut = new LispEnvironment();
        sut.ReadEvaluatePrint("(define foo (lambda (n) (if (= n 0) 0 (bar (- n 1)))))");
        sut.ReadEvaluatePrint("(define bar (lambda (n) (if (= n 0) 0 (foo (- n 1)))))");
        Assert.AreEqual("0", sut.ReadEvaluatePrint("(foo 10000)"));
    }
}

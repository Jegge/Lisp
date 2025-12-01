namespace TestLisp;

using Lisp;

[TestClass]
public sealed class TestMacro
{
    [TestMethod]
    [DataRow("(define-macro one (lambda () 1))", new[] { "(one)" }, new[] { "1" })]
    [DataRow("(define-macro two (lambda () 2))", new[] { "(two)" }, new[] { "2" })]
    [DataRow("(define-macro unless (lambda (pred a b) `(if ~pred ~b ~a)))", new[] { "(unless false 7 8)", "(unless true 7 8)" }, new[] { "7", "8" })]
    [DataRow("(define-macro unless (lambda (pred a b) (list 'if (list 'not pred) a b)))", new[] { "(unless false 7 8)", "(unless true 7 8)" }, new[] { "7", "8" })]
    [DataRow("(define-macro identity (lambda (x) x))", new[] { "(let (a 23) (identity a))" }, new[] { "23" })]
    public void DefineMacro(string macro, string[] input, string[] expected)
    {
        var sut = new LispEnvironment();
        sut.ReadEvaluatePrint(macro);

        foreach (var (i, e) in input.Zip(expected))
            Assert.AreEqual(e, sut.ReadEvaluatePrint(i), "input:<{0}>", [input]);
    }

    [TestMethod]
    public void DefineMacroResult ()
    {
        Assert.AreEqual("true", new LispEnvironment().ReadEvaluatePrint("(let [m (define-macro _ (lambda [] 1))] (macro? m))"));
    }
    [TestMethod]
    public void MacroUsesClosures ()
    {
        var sut = new LispEnvironment();
        Assert.AreEqual("2", sut.ReadEvaluatePrint("(define x 2)"));
        Assert.AreEqual("(lambda () x)", sut.ReadEvaluatePrint("(define-macro a (lambda [] x))"));
        Assert.AreEqual("2", sut.ReadEvaluatePrint("(a)"));
        Assert.AreEqual("2", sut.ReadEvaluatePrint("(let (x 3) (a))"));


    }
}

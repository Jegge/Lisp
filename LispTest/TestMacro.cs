using Lisp;
using Lisp.Types;

namespace LispTest;

[TestClass]
public sealed class TestMacro
{
    [TestMethod]
    [DataRow("(define-macro one (lambda () 1))", new[] { "(one)" }, new[] { "1" })]
    [DataRow("(define-macro two (lambda () 2))", new[] { "(two)" }, new[] { "2" })]
    [DataRow("(define-macro unless (lambda (pred a b) `(if ~pred ~b ~a)))", new[] { "(unless false 7 8)", "(unless true 7 8)" }, new[] { "7", "8" })]
    [DataRow("(define-macro unless (lambda (pred a b) (list 'if (list 'not pred) a b)))", new[] { "(unless false 7 8)", "(unless true 7 8)" }, new[] { "7", "8" })]
    [DataRow("(define-macro identity (lambda (x) x))", new[] { "(let (a 23) (identity a))" }, new[] { "23" })]
    public void DefineMacro (string macro, string[] input, string[] expected)
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

    [TestMethod]
    [DataRow("(cond)", "nil")]
    [DataRow("(cond true 7)", "7")]
    [DataRow("(cond false 7)", "nil")]
    [DataRow("(cond true 7 true 8)", "7")]
    [DataRow("(cond false 7 true 8)", "8")]
    [DataRow("(cond false 7 false 8 \"else\" 9)", "9")]
    [DataRow("(cond false 7 (= 2 2) 8 \"else\" 9)", "8")]
    [DataRow("(cond false 7 false 8 false 9)", "nil")]
    [DataRow("(let (x (cond false \"no\" true \"yes\")) x)", "\"yes\"")]
    [DataRow("(let [x (cond false \"no\" true \"yes\")] x)", "\"yes\"")]
    public void Cond (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(type-of (define-macro test (lambda xs '())))", typeof(LispLambda))]
    [DataRow("(type-of (lambda (x) x))", typeof(LispLambda))]
    [DataRow("(type-of +)", typeof(LispPrimitive))]
    public void TypeOf (string input, Type expected)
    {
        Assert.AreEqual(LispValue.GetLispType(expected), new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(lambda? (define (test) '()))", "true")]
    [DataRow("(lambda? (define-macro test (lambda xs '())))", "true")]
    [DataRow("(lambda? nil)", "false")]
    [DataRow("(lambda? false)", "false")]
    [DataRow("(lambda? true)", "false")]
    [DataRow("(lambda? 0)", "false")]
    [DataRow("(lambda? '())", "false")]
    [DataRow("(lambda? [])", "false")]
    [DataRow("(lambda? {})", "false")]
    [DataRow("(lambda? \"\")", "false")]
    [DataRow("(macro? (define (test) '()))", "false")]
    [DataRow("(macro? (define-macro test (lambda xs '())))", "true")]
    [DataRow("(macro? nil)", "false")]
    [DataRow("(macro? false)", "false")]
    [DataRow("(macro? true)", "false")]
    [DataRow("(macro? 0)", "false")]
    [DataRow("(macro? '())", "false")]
    [DataRow("(macro? [])", "false")]
    [DataRow("(macro? {})", "false")]
    [DataRow("(macro? \"\")", "false")]
    public void IsType (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }
}

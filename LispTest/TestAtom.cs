using Lisp;
using Lisp.Parser;
using Lisp.Types;

namespace LispTest;

[TestClass]
public sealed class TestAtom
{
    [TestMethod]
    public void ReadDeref()
    {
        Assert.AreEqual("(deref a)", LispReader.Read("@a").Print(true));
    }

    [TestMethod]
    [DataRow("(= (atom 23) (atom 23))", "false")]
    [DataRow("(= @(atom 23) @(atom 23))", "true")]
    public void Equality (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(type-of (atom 42))", typeof(LispAtom))]
    public void TypeOf (string input, Type expected)
    {
        Assert.AreEqual(LispValue.GetLispType(expected), new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    public void Mutability ()
    {
        var sut = new LispEnvironment();
        Assert.AreEqual("(atom 2)", sut.ReadEvaluatePrint("(define a (atom 2))"));
        Assert.AreEqual("true", sut.ReadEvaluatePrint("(atom? a)"));
        Assert.AreEqual("false", sut.ReadEvaluatePrint("(atom? 1)"));
        Assert.AreEqual("2", sut.ReadEvaluatePrint("(deref a)"));
        Assert.AreEqual("3", sut.ReadEvaluatePrint("(reset a 3)"));
        Assert.AreEqual("3", sut.ReadEvaluatePrint("(deref a)"));
    }

    [TestMethod]
    public void Swap ()
    {
        var sut = new LispEnvironment();
        sut.ReadEvaluatePrint("(define inc3 (lambda (a) (+ 3 a)))");

        Assert.AreEqual("(atom 3)", sut.ReadEvaluatePrint("(define a (atom 3))"));
        Assert.AreEqual("6", sut.ReadEvaluatePrint("(swap a inc3)"));
        Assert.AreEqual("6", sut.ReadEvaluatePrint("(swap a (lambda (a) a))"));
        Assert.AreEqual("12", sut.ReadEvaluatePrint("(swap a (lambda (a) (* 2 a)))"));
        Assert.AreEqual("120", sut.ReadEvaluatePrint("(swap a (lambda (a b) (* a b)) 10)"));
        Assert.AreEqual("123", sut.ReadEvaluatePrint("(swap a + 3)"));
    }

    [TestMethod]
    [DataRow("(atom? nil)", "false")]
    [DataRow("(atom? false)", "false")]
    [DataRow("(atom? true)", "false")]
    [DataRow("(atom? ())", "false")]
    [DataRow("(atom? {})", "false")]
    [DataRow("(atom? [])", "false")]
    [DataRow("(atom? 0)", "false")]
    [DataRow("(atom? true?)", "false")]
    [DataRow("(atom? (atom 23))", "true")]
    public void IsType (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }
}

using Lisp;

namespace LispTest;

[TestClass]
public sealed class TestEvaluate
{
    [TestMethod]
    [DataRow("1 ; comment after expression", "1")]
    [DataRow(" ; whole line comment (not an exception)\n1 ; comment after expression", "1")]
    [DataRow("1; &()*+,-./:;<=>?@[]^_{|}~", "1")]
    public void SkipsComments(string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(+ 1 2)", "3")]
    [DataRow("(+ 5 (* 2 3))", "11")]
    [DataRow("(- (+ 5 (* 2 3)) 3)", "8")]
    [DataRow("(/ (- (+ 5 (* 2 3)) 3) 4)", "2")]
    [DataRow("(/ (- (+ 515 (* 87 311)) 302) 27)", "1010")]
    [DataRow("(* -3 6)", "-18")]
    [DataRow("(/ (- (+ 515 (* -87 311)) 296) 27)", "-994")]
    public void Arithmetic(string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("()", "()")]
    public void Lists(string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("[nil]", "[nil]")]
    [DataRow("[1 2 (+ 1 2)]", "[1 2 3]")]
    public void Vectors(string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("{}", "{}")]
    [DataRow("{\"a\" (+ 7 8)}", "{\"a\" 15}")]
    [DataRow("{:a (+ 7 8)}", "{:a 15}")]
    public void Hashmaps(string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(apply + (list 2 3))", "5")]
    [DataRow("(apply + 4 (list 5))", "9")]
    [DataRow("(apply strcat (list 1 2 \"3\" (list)))", "\"123()\"")]
    [DataRow("(apply list (list))", "()")]
    [DataRow("(apply symbol? (list (quote two)))", "true")]
    [DataRow("(apply (lambda (a b) (+ a b)) (list 2 3))", "5")]
    [DataRow("(apply (lambda (a b) (+ a b)) 4 (list 5))", "9")]
    [DataRow("(apply m (list 2 3))", "5")]
    [DataRow("(apply m 4 (list 5))", "9")]
    [DataRow("(apply + 4 [5])", "9")]
    [DataRow("(apply strcat [1 2 \"3\" 4])", "\"1234\"")]
    [DataRow("(apply list [])", "()")]
    [DataRow("(apply (lambda (a b) (+ a b)) [2 3])", "5")]
    [DataRow("(apply (lambda (a b) (+ a b)) 4 [5])", "9")]
    public void Apply(string input, string expected)
    {
        var sut = new LispEnvironment();
        sut.ReadEvaluatePrint("(define-macro m (lambda [a b] (+ a b)))");
        Assert.AreEqual(expected, sut.ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    public void EvalUsesGlobalEnvironment ()
    {
        var sut = new LispEnvironment();
        Assert.AreEqual("1", sut.ReadEvaluatePrint("(define a 1)"));
        Assert.AreEqual("1", sut.ReadEvaluatePrint("(let (a 2) (eval (read \"a\")))"));
    }
}

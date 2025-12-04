using Lisp;
using Lisp.Parser;
using Lisp.Types;

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
    public void CommaAsWhitespace ()
    {
        Assert.AreEqual("(1 2 3)", LispReader.Read("(1 2, 3, ,,,,),,").Print(true));
    }
    
    [TestMethod]
    [DataRow("(not false)", "true")]
    [DataRow("(not nil)", "true")]
    [DataRow("(not true)", "false")]
    [DataRow("(not \"a\")", "false")]
    [DataRow("(not 0)", "false")]
    public void Not (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(read \"(1 2 (3 4) nil)\")", "(1 2 (3 4) nil)")]
    [DataRow("(= nil (read \"nil\"))", "true")]
    [DataRow("(read \"(+ 2 3)\")", "(+ 2 3)")]
    [DataRow("(read \"\\\"\\n\\\"\")", "\"\\n\"")]
    [DataRow("(read \"7 ;; comment\")", "7")]
    public void ReadString (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(print)", "\"\"")]
    [DataRow("(print \"\")", "\"\\\"\\\"\"")]
    [DataRow("(print \"abc\")", "\"\\\"abc\\\"\"")]
    [DataRow("(print \"abc  def\" \"ghi jkl\")", "\"\\\"abc  def\\\" \\\"ghi jkl\\\"\"")]
    [DataRow("(print \"\\\"\")", "\"\\\"\\\\\\\"\\\"\"")]
    [DataRow("(print (list 1 2 \"abc\" \"\\\"\") \"def\")", "\"(1 2 \\\"abc\\\" \\\"\\\\\\\"\\\") \\\"def\\\"\"")]
    [DataRow("(print \"abc\\ndef\\nghi\")", "\"\\\"abc\\\\ndef\\\\nghi\\\"\"")]
    [DataRow("(print \"abc\\\\def\\\\ghi\")", "\"\\\"abc\\\\\\\\def\\\\\\\\ghi\\\"\"")]
    [DataRow("(print [1 2 \"abc\" \"\\\"\"] \"def\")", "\"[1 2 \\\"abc\\\" \\\"\\\\\\\"\\\"] \\\"def\\\"\"")]
    [DataRow("(print [])", "\"[]\"")]
    public void Print (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }
    [TestMethod]
    [DataRow("(strcat)", "\"\"")]
    [DataRow("(strcat \"\")", "\"\"")]
    [DataRow("(strcat \"abc\")", "\"abc\"")]
    [DataRow("(strcat \"\\\"\")", "\"\\\"\"")]
    [DataRow("(strcat 1 \"abc\" 3)", "\"1abc3\"")]
    [DataRow("(strcat \"abc  def\" \"ghi jkl\")", "\"abc  defghi jkl\"")]
    [DataRow("(strcat \"abc\\ndef\\nghi\")", "\"abc\\ndef\\nghi\"")]
    [DataRow("(strcat \"abc\\\\def\\\\ghi\")", "\"abc\\\\def\\\\ghi\"")]
    [DataRow("(strcat (list 1 2 \"abc\" \"\\\"\") \"def\")", "\"(1 2 abc \\\")def\"")]
    [DataRow("(strcat (list))", "\"()\"")]
    [DataRow("(strcat [1 2 \"abc\" \"\\\"\"] \"def\")", "\"[1 2 abc \\\"]def\"")]
    [DataRow("(strcat [])", "\"[]\"")]
    public void Strcat (string input, string expected)
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
    public void ApplyTypeMismatchException ()
    {
        Assert.ThrowsException<TypeMismatchException<LispPrimitive, LispLambda>>(() => new LispEnvironment().ReadEvaluatePrint("(apply :a '(1 2 3))"));
    }

    [TestMethod]
    public void EvalUsesGlobalEnvironment ()
    {
        var sut = new LispEnvironment();
        Assert.AreEqual("1", sut.ReadEvaluatePrint("(define a 1)"));
        Assert.AreEqual("1", sut.ReadEvaluatePrint("(let (a 2) (eval (read \"a\")))"));
    }

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

    [TestMethod]
    public void Quine ()
    {
        const string quine = "((lambda (q) (quasiquote ((unquote q) (quote (unquote q))))) (quote (lambda (q) (quasiquote ((unquote q) (quote (unquote q)))))))";
        Assert.AreEqual(quine, new LispEnvironment().ReadEvaluatePrint(quine));
    }

    [TestMethod]
    public void SymbolNotFoundException()
    {
        Assert.ThrowsException<SymbolNotFoundException>(() => new LispEnvironment().ReadEvaluatePrint("(abc 1 2 3)"));
    }

    [TestMethod]
    public void CommandLineArguments ()
    {
        Assert.AreEqual("()", new LispEnvironment().ReadEvaluatePrint("argv"));
        Assert.AreEqual("(\"foo\")", new LispEnvironment(["foo"]).ReadEvaluatePrint("argv"));
        Assert.AreEqual("(\"foo\" \"bar\")", new LispEnvironment(["foo", "bar"]).ReadEvaluatePrint("argv"));
    }
}

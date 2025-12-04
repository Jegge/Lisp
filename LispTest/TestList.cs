using Lisp;
using Lisp.Parser;
using Lisp.Types;

namespace LispTest;

[TestClass]
public sealed class TestList
{
    [TestMethod]
    [DataRow("(1 2)", "(1 2)")]
    [DataRow("( )", "()")]
    [DataRow("(nil)", "(nil)")]
    [DataRow("  ( +   1   (+   2 3   )   )  ", "(+ 1 (+ 2 3))")]
    [DataRow("(* 3 4)", "(* 3 4)")]
    [DataRow("(** 3 4)", "(** 3 4)")]
    [DataRow("(-3 4)", "(-3 4)")]
    [DataRow("(1 . 2)", "(1 . 2)")]
    public void ReadAndPrint (string input, string expected)
    {
        Assert.AreEqual(expected, LispReader.Read(input).Print(true), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(1 2")]
    [DataRow(")")]
    [DataRow(") (+ 1 2)")]
    [DataRow("(1 . . 2)")]
    [DataRow("(1 . )")]
    [DataRow("( . 2)")]
    public void UnbalancedParenthesisException(string input)
    {
        Assert.ThrowsException<UnbalancedParenthesisException>(() => LispReader.Read(input).Print(true));
    }

    [TestMethod]
    [DataRow("(list)", "()")]
    [DataRow("(list 1 2 3)", "(1 2 3)")]
    public void Construction (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(list? nil)", "false")]
    [DataRow("(list? '())", "true")]
    [DataRow("(list? '(1 2 3))", "true")]
    [DataRow("(list? [])", "false")]
    [DataRow("(list? 2)", "false")]
    [DataRow("(list? \"a\")", "false")]
    [DataRow("(list? 'a)", "false")]
    [DataRow("(list? :a)", "false")]
    [DataRow("(list? false)", "false")]
    [DataRow("(sequential? '(1 2 3))", "true")]
    [DataRow("(sequential? (list 1 2 3))", "true")]
    public void IsList (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(null? '())", "true")]
    [DataRow("(null? (list))", "true")]
    [DataRow("(null? (list 1))", "false")]
    public void IsNull (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(length (list 1 2 3))", "3")]
    [DataRow("(length '(1 2 3))", "3")]
    [DataRow("(length (list))", "0")]
    [DataRow("(length '())", "0")]
    public void Length (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(= (list) (list))", "true")]
    [DataRow("(= (list) ())", "true")]
    [DataRow("(= (list 1 2) (list 1 2))", "true")]
    [DataRow("(= (list 1) (list))", "false")]
    [DataRow("(= (list) (list 1))", "false")]
    [DataRow("(= 0 (list))", "false")]
    [DataRow("(= (list) 0)", "false")]
    [DataRow("(= (list nil) (list))", "false")]
    [DataRow("(= (list) nil)", "false")]
    [DataRow("(= (list) \"\")", "false")]
    [DataRow("(= \"\" (list))", "false")]
    [DataRow("(= (list :abc) (list :abc))", "true")]
    [DataRow("(= [] (list))", "true")]
    [DataRow("(= (list 1 2) [1 2])", "true")]
    [DataRow("(= (list 1) [])", "false")]
    [DataRow("(= [(list)] (list []))", "true")]
    [DataRow("(= [1 2 (list 3 4 [5 6])] (list 1 2 [3 4 (list 5 6)]))", "true")]
    public void Equality (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(print (list))", "\"()\"")]
    [DataRow("(print (list 1 2 3))", "\"(1 2 3)\"")]
    public void Print (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(type-of (list))", typeof(LispList))]
    [DataRow("(type-of (list 1 2 3))", typeof(LispList))]
    public void TypeOf (string input, Type expected)
    {
        Assert.AreEqual(LispValue.GetLispType(expected), new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(cons 1 (list))", "(1)")]
    [DataRow("(cons 1 (list 2))", "(1 2)")]
    [DataRow("(cons 1 (list 2 3))", "(1 2 3)")]
    [DataRow("(cons (list 1) (list 2 3))", "((1) 2 3)")]
    [DataRow("(list (cons 1 a) a) ", "((1 2 3) (2 3))")]
    public void Cons (string input, string expected)
    {
        var sut = new LispEnvironment();
        sut.ReadEvaluatePrint("(define a (list 2 3))");
        Assert.AreEqual(expected, sut.ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(concat)", "()")]
    [DataRow("(concat (list 1 2))", "(1 2)")]
    [DataRow("(concat (list 1 2) (list 3 4))", "(1 2 3 4)")]
    [DataRow("(concat (list 1 2) (list 3 4) (list 5 6))", "(1 2 3 4 5 6)")]
    [DataRow("(concat (concat))", "()")]
    [DataRow("(concat (list) (list))", "()")]
    [DataRow("(= () (concat))", "true")]
    public void Concat (string input, string expected)
    {
        var sut = new LispEnvironment();
        sut.ReadEvaluatePrint("(define a (list 2 3))");
        sut.ReadEvaluatePrint("(define b (list 4 5))");
        Assert.AreEqual(expected, sut.ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(first (list))", "nil")]
    [DataRow("(first (list 6))", "6")]
    [DataRow("(first (list 6 7 8))", "6")]
    public void First (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(rest (list))", "()")]
    [DataRow("(rest (list 6))", "()")]
    [DataRow("(rest (list 6 7 8))", "(7 8)")]
    public void Rest (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(nth (list 1) 0)", "1")]
    [DataRow("(nth (list 1 2) 1)", "2")]
    [DataRow("(nth (list 1 2 nil) 2)", "nil")]
    public void Nth (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(map double numbers)", "(2 4 6)")]
    [DataRow("(map (lambda (x) (symbol? x)) (list 1 (quote two) \"three\"))", "(false true false)")]
    [DataRow("(= () (map strcat ()))", "true")]
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
    [DataRow("(filter (lambda (x) (= (mod x 2) 0)) '(1 2 3 4 5 6 7 8 9 10))", "(2 4 6 8 10)")]
    public void Filter (string input, string expected)
    {
        var sut = new LispEnvironment();
        sut.ReadEvaluatePrint("(define numbers (list 1 2 3 4 5 6 7 8 9 10))");
        Assert.AreEqual(expected, sut.ReadEvaluatePrint(input), "input:<{0}>", input);
    }


    [TestMethod]
    [DataRow("(contains? '() :a)", "false")]
    [DataRow("(contains? '(:b) :a)", "false")]
    [DataRow("(contains? '(:a :b :c) :a)", "true")]
    [DataRow("(contains? '(:a :b :c) :b)", "true")]
    [DataRow("(contains? '(:a :b :c) :c)", "true")]
    [DataRow("(contains? '(:a :b :c) :d)", "false")]
    public void Contains(string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }
}

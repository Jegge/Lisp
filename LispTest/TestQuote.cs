using Lisp;

namespace LispTest;

[TestClass]
public sealed class TestQuote
{
    [TestMethod]
    [DataRow("(quote 7)", "7")]
    [DataRow("(quote (1 2 3))", "(1 2 3)")]
    [DataRow("(quote abc)", "abc")]
    [DataRow("(quote (1 2 (3 4)))", "(1 2 (3 4))")]
    public void Quote(string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(quasiquote nil)", "nil")]
    [DataRow("(quasiquote 7)", "7")]
    [DataRow("(quasiquote a)", "a")]
    [DataRow("(quasiquote {\"a\" b})", "{\"a\" b}")]
    [DataRow("(quasiquote 7)", "7")]
    [DataRow("(quasiquote 7)", "7")]
    [DataRow("(quasiquote ())", "()")]
    [DataRow("(quasiquote (1 2 3))", "(1 2 3)")]
    [DataRow("(quasiquote (a))", "(a)")]
    [DataRow("(quasiquote (1 2 (3 4)))", "(1 2 (3 4))")]
    [DataRow("(quasiquote (nil))", "(nil)")]
    [DataRow("(quasiquote (1 ()))", "(1 ())")]
    [DataRow("(quasiquote (() 1))", "(() 1)")]
    [DataRow("(quasiquote (1 () 2))", "(1 () 2)")]
    [DataRow("(quasiquote (()))", "(())")]
    [DataRow("(quasiquote [])", "[]")]
    [DataRow("(quasiquote [[]])", "[[]]")]
    [DataRow("(quasiquote [()])", "[()]")]
    [DataRow("(quasiquote [()])", "[()]")]
    [DataRow("(quasiquote [1 a 3])", "[1 a 3]")]
    [DataRow("(quasiquote [a [] b [c] d [e f] g])", "[a [] b [c] d [e f] g]")]

    public void Quasiquote(string input, string expected)
    {
        var sut = new LispEnvironment();
        sut.ReadEvaluatePrint("(define a 8)");
        Assert.AreEqual(expected, sut.ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(quasiquote (unquote 7))", "7")]
    [DataRow("(quasiquote a)", "a")]
    [DataRow("(quasiquote (unquote a))", "8")]
    [DataRow("(quasiquote (1 a 3))", "(1 a 3)")]
    [DataRow("(quasiquote (1 (unquote a) 3))", "(1 8 3)")]
    [DataRow("(quasiquote (1 b 3))", "(1 b 3)")]
    [DataRow("(quasiquote (1 (unquote b) 3))", "(1 (1 \"b\" \"d\") 3)")]
    [DataRow("(quasiquote ((unquote 1) (unquote 2)))", "(1 2)")]
    [DataRow("(let (x 0) (quasiquote (unquote x)))", "0")]
    [DataRow("`[~a]", "[8]")]
    [DataRow("`[(~a)]", "[(8)]")]
    [DataRow("`([~a])", "([8])")]
    [DataRow("`[a ~a a]", "[a 8 a]")]
    [DataRow("`[(a ~a a)]", "[(a 8 a)]")]
    [DataRow("`(0 unquote)", "(0 unquote)")]
    [DataRow("`[0 unquote]", "[0 unquote]")]
    [DataRow("`(0 unquote 1)", "(0 unquote 1)")]
    public void Unquote(string input, string expected)
    {
        var sut = new LispEnvironment();
        sut.ReadEvaluatePrint("(define a 8)");
        sut.ReadEvaluatePrint("(define b (quote (1 \"b\" \"d\")))");
        sut.ReadEvaluatePrint("(define c (quote (1 \"b\" \"d\")))");
        Assert.AreEqual(expected, sut.ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(quasiquote (1 c 3))", "(1 c 3)")]
    [DataRow("(quasiquote (1 (splice-unquote c) 3))", "(1 1 \"b\" \"d\" 3)")]
    [DataRow("(quasiquote (1 (splice-unquote c)))", "(1 1 \"b\" \"d\")")]
    [DataRow("(quasiquote ((splice-unquote c) 2))", "(1 \"b\" \"d\" 2)")]
    [DataRow("(quasiquote ((splice-unquote c) (splice-unquote c)))", "(1 \"b\" \"d\" 1 \"b\" \"d\")")]
    [DataRow("`[~@c]", "[1 \"b\" \"d\"]")]
    [DataRow("`[(~@c)]", "[(1 \"b\" \"d\")]")]
    [DataRow("`([~@c])", "([1 \"b\" \"d\"])")]
    [DataRow("`[1 ~@c 3]", "[1 1 \"b\" \"d\" 3]")]
    [DataRow("`([1 ~@c 3])", "([1 1 \"b\" \"d\" 3])")]
    [DataRow("`[(1 ~@c 3)]", "[(1 1 \"b\" \"d\" 3)]")]
    [DataRow("`(0 splice-unquote)", "(0 splice-unquote)")]
    [DataRow("`[0 splice-unquote]", "[0 splice-unquote]")]
    [DataRow("`(0 splice-unquote ())", "(0 splice-unquote ())")]
    public void SpliceUnquote(string input, string expected)
    {
        var sut = new LispEnvironment();
        sut.ReadEvaluatePrint("(define c (quote (1 \"b\" \"d\")))");
        Assert.AreEqual(expected, sut.ReadEvaluatePrint(input), "input:<{0}>", input);
    }
}

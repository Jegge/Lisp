using Lisp;
using Lisp.Types;

namespace LispTest;

[TestClass]
public sealed class TestBuiltin
{
    [TestMethod]
    [DataRow("(> 2 1)", "true")]
    [DataRow("(> 1 1)", "false")]
    [DataRow("(> 1 2)", "false")]
    [DataRow("(>= 2 1)", "true")]
    [DataRow("(>= 1 1)", "true")]
    [DataRow("(>= 1 2)", "false")]
    [DataRow("(< 2 1)", "false")]
    [DataRow("(< 1 1)", "false")]
    [DataRow("(< 1 2)", "true")]
    [DataRow("(<= 2 1)", "false")]
    [DataRow("(<= 1 1)", "true")]
    [DataRow("(<= 1 2)", "true")]
    public void Conditionals (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(= 2 1)", "false")]
    [DataRow("(= 1 1)", "true")]
    [DataRow("(= 1 2)", "false")]
    [DataRow("(= 1 (+ 1 1))", "false")]
    [DataRow("(= 2 (+ 1 1))", "true")]
    [DataRow("(= nil nil)", "true")]
    [DataRow("(= nil false)", "false")]
    [DataRow("(= nil true)", "false")]
    [DataRow("(= nil 0)", "false")]
    [DataRow("(= nil 1)", "false")]
    [DataRow("(= nil \"\")", "false")]
    [DataRow("(= nil ())", "false")]
    [DataRow("(= nil [])", "false")]
    [DataRow("(= nil {})", "false")]
    [DataRow("(= false nil)", "false")]
    [DataRow("(= false false)", "true")]
    [DataRow("(= false true)", "false")]
    [DataRow("(= false 0)", "false")]
    [DataRow("(= false 1)", "false")]
    [DataRow("(= false \"\")", "false")]
    [DataRow("(= false ())", "false")]
    [DataRow("(= false [])", "false")]
    [DataRow("(= false {})", "false")]
    [DataRow("(= true nil)", "false")]
    [DataRow("(= true false)", "false")]
    [DataRow("(= true true)", "true")]
    [DataRow("(= true 0)", "false")]
    [DataRow("(= true 1)", "false")]
    [DataRow("(= true \"\")", "false")]
    [DataRow("(= true ())", "false")]
    [DataRow("(= true [])", "false")]
    [DataRow("(= true {})", "false")]
    [DataRow("(= \"\" \"\")", "true")]
    [DataRow("(= \"abc\" \"abc\")", "true")]
    [DataRow("(= \"abc\" \"\")", "false")]
    [DataRow("(= \"\" \"abc\")", "false")]
    [DataRow("(= \"abc\" \"def\")", "false")]
    [DataRow("(= \"abc\" \"ABC\")", "false")]
    [DataRow("(= (list) \"\")", "false")]
    [DataRow("(= \"\" (list))", "false")]
    [DataRow("(= :abc :abc)", "true")]
    [DataRow("(= :abc :def)", "false")]
    [DataRow("(= :abc \":abc\")", "false")]
    [DataRow("(= (list :abc) (list :abc))", "true")]
    [DataRow("(= (list 1 2) [1 2])", "true")]
    [DataRow("(= (list 1) [])", "false")]
    [DataRow("(= [(list)] (list []))", "true")]
    [DataRow("(= [1 2 (list 3 4 [5 6])] (list 1 2 [3 4 (list 5 6)]))", "true")]
    [DataRow("(= (atom 23) (atom 23))", "false")]
    [DataRow("(= @(atom 23) @(atom 23))", "true")]
    public void Equality (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
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
    [DataRow("(type-of nil)", typeof(LispNil))]
    [DataRow("(type-of true)", typeof(LispBool))]
    [DataRow("(type-of false)", typeof(LispBool))]
    [DataRow("(type-of 23)", typeof(LispNumber))]
    [DataRow("(type-of 0)", typeof(LispNumber))]
    [DataRow("(type-of -42)", typeof(LispNumber))]
    [DataRow("(type-of {})", typeof(LispHashMap))]
    [DataRow("(type-of \"abc\")", typeof(LispString))]
    [DataRow("(type-of 'abc)", typeof(LispSymbol))]
    [DataRow("(type-of :abc)", typeof(LispKeyword))]
    [DataRow("(type-of (lambda (x) x))", typeof(LispLambda))]
    [DataRow("(type-of +)", typeof(LispPrimitive))]
    [DataRow("(type-of (atom 42))", typeof(LispAtom))]
    public void TypeOf (string input, Type expected)
    {
        Assert.AreEqual(LispValue.GetLispType(expected), new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    public void Atom ()
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
    [DataRow("(symbol? 'abc)", "true")]
    [DataRow("(symbol? \"abc\")", "false")]
    [DataRow("(nil? nil)", "true")]
    [DataRow("(nil? false)", "false")]
    [DataRow("(nil? true)", "false")]
    [DataRow("(nil? ())", "false")]
    [DataRow("(nil? 0)", "false")]
    [DataRow("(true? nil)", "false")]
    [DataRow("(true? false)", "false")]
    [DataRow("(true? true)", "true")]
    [DataRow("(true? ())", "false")]
    [DataRow("(true? 0)", "false")]
    [DataRow("(true? true?)", "false")]
    [DataRow("(false? nil)", "false")]
    [DataRow("(false? false)", "true")]
    [DataRow("(false? true)", "false")]
    [DataRow("(false? 0)", "false")]
    [DataRow("(false? ())", "false")]
    [DataRow("(false? [])", "false")]
    [DataRow("(false? {})", "false")]
    [DataRow("(false? \"\")", "false")]
    [DataRow("(symbol? :abc)", "false")]
    [DataRow("(symbol? 'abc)", "true")]
    [DataRow("(symbol? \"abc\")", "false")]
    [DataRow("(symbol? (symbol \"abc\"))", "true")]
    [DataRow("(keyword? :abc)", "true")]
    [DataRow("(keyword? 'abc)", "false")]
    [DataRow("(keyword? \"abc\")", "false")]
    [DataRow("(keyword? \"\")", "false")]
    [DataRow("(keyword? (keyword \"abc\"))", "true")]
    [DataRow("(sequential? (list 1 2 3))", "true")]
    [DataRow("(sequential? [15])", "true")]
    [DataRow("(sequential? sequential?)", "false")]
    [DataRow("(sequential? nil)", "false")]
    [DataRow("(sequential? \"abc\")", "false")]
    [DataRow("(hashmap? {})", "true")]
    [DataRow("(hashmap? '())", "false")]
    [DataRow("(hashmap? [])", "false")]
    [DataRow("(hashmap? 'abc)", "false")]
    [DataRow("(hashmap? :abc)", "false")]
    [DataRow("(string? nil)", "false")]
    [DataRow("(string? \"\")", "true")]
    [DataRow("(string? \"test\")", "true")]
    [DataRow("(string? 2)", "false")]
    [DataRow("(string? (list))", "false")]
    [DataRow("(string? [])", "false")]
    [DataRow("(string? :abc)", "false")]
    [DataRow("(number? nil)", "false")]
    [DataRow("(number? 2)", "true")]
    [DataRow("(number? 4.2)", "true")]
    [DataRow("(number? \"\")", "false")]
    [DataRow("(number? \"abc\")", "false")]
    [DataRow("(number? (list))", "false")]
    [DataRow("(number? [])", "false")]
    [DataRow("(number? :abc)", "false")]

    //[DataRow("", "")]
    //[DataRow("", "")]
    public void IsType (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(symbol \"abc\")", "abc")]
    [DataRow("(keyword \"abc\")", ":abc")]
    public void Symbol (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }
}

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
    [DataRow("(= (list 1 2) [1 2])", "true")]
    [DataRow("(= (list 1) [])", "false")]
    [DataRow("(= [(list)] (list []))", "true")]
    [DataRow("(= [1 2 (list 3 4 [5 6])] (list 1 2 [3 4 (list 5 6)]))", "true")]
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
    [DataRow("(type-of \"abc\")", typeof(LispString))]
    [DataRow("(type-of (lambda (x) x))", typeof(LispLambda))]
    [DataRow("(type-of +)", typeof(LispPrimitive))]
    public void TypeOf (string input, Type expected)
    {
        Assert.AreEqual(LispValue.GetLispType(expected), new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
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
    public void IsType (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }
}

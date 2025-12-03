using Lisp;
using Lisp.Types;

namespace LispTest;

[TestClass]
public sealed class TestHashMap
{
    [TestMethod]
    [DataRow("{}", "{}")]
    [DataRow("{ }", "{}")]
    [DataRow("{\"abc\" 1}", "{\"abc\" 1}")]
    [DataRow("{\"a\" { \"b\" {\"c\" 1}}}", "{\"a\" {\"b\" {\"c\" 1}}}")]
    [DataRow("{\"a\" { \"b\" {\"cde\" 3}}}", "{\"a\" {\"b\" {\"cde\" 3}}}")]
    [DataRow("{\"a1\" 1 \"a2\" 2 \"a3\" 3}", "{\"a1\" 1 \"a2\" 2 \"a3\" 3}")]
    [DataRow("{:a { :b {:c 3}}}", "{:a {:b {:c 3}}}")]
    [DataRow("{\"1\" 1}", "{\"1\" 1}")]
    [DataRow("({})", "({})")]
    public void ReadAndPrint (string input, string expected)
    {
        Assert.AreEqual(expected, LispValue.Read(input).Print(true), "input:<{0}>", input);
    }
    
    [TestMethod]
    [DataRow("{\"abc\" 2")]
    [DataRow("}")]
    [DataRow("} (+ 1 2)")]
    public void UnbalancedParenthesisException(string input)
    {
        Assert.ThrowsException<UnbalancedParenthesisException>(() => LispValue.Read(input).Print(true));
    }

    [TestMethod]
    [DataRow("{}", "{}")]
    [DataRow("{\"a\" (+ 7 8)}", "{\"a\" 15}")]
    [DataRow("{:a (+ 7 8)}", "{:a 15}")]
    public void Construction (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(= {} nil)", "false")]
    [DataRow("(= {} false)", "false")]
    [DataRow("(= {} true)", "false")]
    [DataRow("(= {} 0)", "false")]
    [DataRow("(= {} 1)", "false")]
    [DataRow("(= {} \"\")", "false")]
    [DataRow("(= {} ())", "false")]
    [DataRow("(= {} [])", "false")]
    [DataRow("(= {} {})", "true")]
    public void Equality (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(null? {})", "true")]
    [DataRow("(null? { :a 23 })", "false")]
    public void IsNull (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(length {})", "0")]
    [DataRow("(length { :a 23 :b 42 })", "2")]
    public void Length (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(type-of {})", typeof(LispHashMap))]
    [DataRow("(type-of { :a 23 })", typeof(LispHashMap))]
    public void TypeOf (string input, Type expected)
    {
        Assert.AreEqual(LispValue.GetLispType(expected), new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(hashmap? {})", "true")]
    [DataRow("(hashmap? '())", "false")]
    [DataRow("(hashmap? [])", "false")]
    [DataRow("(hashmap? 'abc)", "false")]
    [DataRow("(hashmap? :abc)", "false")]
    public void IsType (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }
}

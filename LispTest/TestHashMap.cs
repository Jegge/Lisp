using Lisp;
using Lisp.Parser;
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
        Assert.AreEqual(expected, LispReader.Read(input).Print(true), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("{\"abc\" 2")]
    [DataRow("}")]
    [DataRow("} (+ 1 2)")]
    public void UnbalancedParenthesisException(string input)
    {
        Assert.ThrowsException<UnbalancedParenthesisException>(() => LispReader.Read(input).Print(true));
    }

    [TestMethod]
    [DataRow("{\"abc\" }")]
    [DataRow("{\"abc\"")]
    public void UnexpectedEndOfInputException (string input)
    {
        Assert.ThrowsException<UnexpectedEndOfInputException>(() => LispReader.Read(input).Print(true));
    }


    [TestMethod]
    [DataRow("{}", "{}")]
    [DataRow("{\"a\" (+ 7 8)}", "{\"a\" 15}")]
    [DataRow("{:a (+ 7 8)}", "{:a 15}")]
    [DataRow("(hashmap)", "{}")]
    [DataRow("(hashmap :a 42)", "{:a 42}")]
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

    [TestMethod]
    [DataRow("(contains-key? {} :a)", "false")]
    [DataRow("(contains-key? { :b 23 } :a)", "false")]
    [DataRow("(contains-key? { :b 23 } :b)", "true")]
    public void ContainsKey (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(contains? {} :a)", "false")]
    [DataRow("(contains? { :b 23 } :a)", "false")]
    [DataRow("(contains? { :b 23 } :b)", "false")]
    [DataRow("(contains? { :b 23 } 23)", "true")]
    public void Contains(string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(assoc {})", "{}")]
    [DataRow("(assoc {} :a 23)", "{:a 23}")]
    [DataRow("(assoc { :a 23 })", "{:a 23}")]
    [DataRow("(assoc { :a 23 } :b 42)", "{:a 23 :b 42}")]
    [DataRow("(assoc { :a 23 } :b 42 :c 47)", "{:a 23 :b 42 :c 47}")]
    public void Assoc(string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }
    [TestMethod]
    [DataRow("(dissoc {})", "{}")]
    [DataRow("(dissoc {} :a)", "{}")]
    [DataRow("(dissoc { :a 23 })", "{:a 23}")]
    [DataRow("(dissoc { :a 23 } :b)", "{:a 23}")]
    [DataRow("(dissoc { :a 23 :b 42 } :b)", "{:a 23}")]
    [DataRow("(dissoc { :a 23 :b 42 } :a :b)", "{}")]
    [DataRow("(dissoc { :a 23 :b 42 } :a :b :c)", "{}")]
    public void Dissoc(string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(keys {})", "()")]
    [DataRow("(keys { :a 23 })", "(:a)")]
    [DataRow("(keys { :a 23 :b 42 })", "(:a :b)")]
    public void Keys (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(values {})", "()")]
    [DataRow("(values { :a 23 })", "(23)")]
    [DataRow("(values { :a 23 :b 42 })", "(23 42)")]
    public void Values (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(get {} :a)", "nil")]
    [DataRow("(get { :a 23 } :a)", "23")]
    [DataRow("(get { :a 23 :b 42 } :b)", "42")]
    [DataRow("(get { :a 23 :b 42 } :c)", "nil")]
    public void Get (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

}

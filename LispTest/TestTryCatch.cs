using Lisp;

namespace LispTest;

[TestClass]
public sealed class TestTryCatch
{
    [TestMethod]
    [DataRow("(try 123 (catch e 456))", "123")]
    [DataRow("(try abc (catch e 456))", "456")]
    [DataRow("(try (list 1) (catch e (prn \"exc is:\" e)))", "(1)")]
    [DataRow("(try (throw \"my exception\") (catch e (do e 7)))", "7")]
    [DataRow("(try (throw \"my exception\") (catch e \"blubb\"))", "\"blubb\"")]
    [DataRow("(try (do (try \"t1\" (catch e \"c1\")) (throw \"e1\")) (catch e \"c2\"))", "\"c2\"")]
    [DataRow("(try (try (throw \"e1\") (catch e (throw \"e2\"))) (catch e \"c2\"))", "\"c2\"")]
    [DataRow("(try (map throw (list \"my err\")) (catch exc exc))", "\"Runtime error: my err.\"")]
    //[DataRow("", "")]


    public void TryCatch (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    public void Throw ()
    {
        Assert.ThrowsException<RuntimeException>(() => new LispEnvironment().ReadEvaluatePrint("(throw \"test\")"));
    }
}

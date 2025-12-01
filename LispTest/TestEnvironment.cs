using Lisp;

namespace LispTest;

[TestClass]
public sealed class TestEnvironment
{
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

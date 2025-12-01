using Lisp.Types;

namespace TestLisp;

using Lisp;

[TestClass]
public sealed class TestFileIO
{
    [TestMethod]
    [DataRow("(read \"(1 2 (3 4) nil)\")", "(1 2 (3 4) nil)")]
    [DataRow("(= nil (read \"nil\"))", "true")]
    [DataRow("(read \"(+ 2 3)\")", "(+ 2 3)")]
    [DataRow("(read \"\\\"\\n\\\"\")", "\"\\n\"")]
    [DataRow("(read \"7 ;; comment\")", "7")]
    //[DataRow("", "")]
    //[DataRow("", "")]
    public void ReadString (string input, string expected)
    {
        var sut = new LispEnvironment(LispAccess.ReadFiles);
        Assert.AreEqual(expected, sut.ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    public void UnexpectedEndOfFileException ()
    {
        Assert.ThrowsException<UnexpectedEndOfFileException>(() => new LispEnvironment(LispAccess.ReadFiles).ReadEvaluatePrint("(read \";; comment\")"));
    }

    [TestMethod]
    [DataRow("./Testfiles/test.txt")]
    [DataRow("./Testfiles/test.txt")]
    [DataRow("./Testfiles/test1.lisp")]
    public void Slurp (string filepath)
    {
        var expected = $"\"{LispString.Escape(File.ReadAllText(filepath))}\"";
        Assert.AreEqual(expected, new LispEnvironment(LispAccess.ReadFiles).ReadEvaluatePrint($"(slurp \"{LispString.Escape(filepath)}\")"), "input:<{0}>", filepath);
    }

    [TestMethod]
    public void SlurpAccessDeniedException ()
    {
        Assert.ThrowsException<AccessDeniedException>(() => new LispEnvironment().ReadEvaluatePrint("(slurp \"blubb\")"));
    }

    [TestMethod]
    [DataRow("./Testfiles/test1.lisp")]
    public void LoadFile (string filepath)
    {
        var sut = new LispEnvironment(LispAccess.ReadFiles);
        sut.LoadFile(filepath);

        Assert.AreEqual("6", sut.ReadEvaluatePrint("(inc4 2)"));
        Assert.AreEqual("8", sut.ReadEvaluatePrint("(inc5 3)"));
    }

    [TestMethod]
    public void EvalUsesGlobalEnvironment ()
    {
        var sut = new LispEnvironment();
        Assert.AreEqual("1", sut.ReadEvaluatePrint("(define a 1)"));
        Assert.AreEqual("1", sut.ReadEvaluatePrint("(let (a 2) (eval (read \"a\")))"));
    }
}

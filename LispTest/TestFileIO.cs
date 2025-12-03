using Lisp.Types;
using Lisp;

namespace LispTest;

[TestClass]
public sealed class TestFileIO
{
    [TestMethod]
    public void UnexpectedEndOfFileException ()
    {
        Assert.ThrowsException<UnexpectedEndOfInputException>(() => new LispEnvironment(LispAccess.ReadFiles).ReadEvaluatePrint("(read \";; comment\")"));
    }

    [TestMethod]
    [DataRow("./Testfiles/test.txt")]
    [DataRow("./Testfiles/test.txt")]
    [DataRow("./Testfiles/test1.lisp")]
    public void FileReadContent (string filepath)
    {
        var expected = $"\"{LispString.Escape(File.ReadAllText(filepath))}\"";
        Assert.AreEqual(expected, new LispEnvironment(LispAccess.ReadFiles).ReadEvaluatePrint($"(file-read-content \"{LispString.Escape(filepath)}\")"), "input:<{0}>", filepath);
    }

    [TestMethod]
    public void FileReadContentAccessDeniedException ()
    {
        Assert.ThrowsException<AccessDeniedException>(() => new LispEnvironment().ReadEvaluatePrint("(file-read-content \"blubb\")"));
    }

    [TestMethod]
    public void OpenCheckCloseFile ()
    {
        var sut = new LispEnvironment(LispAccess.ReadFiles | LispAccess.WriteFiles);

        sut.ReadEvaluatePrint("(define f (file-open-read \"./Testfiles/test.txt\"))");
        Assert.AreEqual("true", sut.ReadEvaluatePrint("(input-file? f)"));
        Assert.AreEqual("false", sut.ReadEvaluatePrint("(output-file? f)"));
        sut.ReadEvaluatePrint("(file-close f)");

        sut.ReadEvaluatePrint("(define f (file-open-write \"./Testfiles/out.txt\"))");
        Assert.AreEqual("false", sut.ReadEvaluatePrint("(input-file? f)"));
        Assert.AreEqual("true", sut.ReadEvaluatePrint("(output-file? f)"));
        sut.ReadEvaluatePrint("(file-close f)");

        File.Delete("./Testfiles/out.txt");
    }

    [TestMethod]
    public void FileWriteContent ()
    {
        var filepath = Path.GetTempFileName();
        const string data = "Das ist ein Test!\n";
        Assert.AreEqual("true", new LispEnvironment(LispAccess.WriteFiles).ReadEvaluatePrint($"(file-write-content \"{LispString.Escape(filepath)}\" \"{LispString.Escape(data)}\")"), "input:<{0}>", filepath);
        Assert.AreEqual("\"Das ist ein Test!\\n\"", new LispEnvironment(LispAccess.ReadFiles).ReadEvaluatePrint($"(file-read-content \"{LispString.Escape(filepath)}\")"), "input:<{0}>", filepath);
        File.Delete(filepath);
    }

    [TestMethod]
    public void FileWriteContentAccessDeniedException ()
    {
        Assert.ThrowsException<AccessDeniedException>(() => new LispEnvironment().ReadEvaluatePrint("(file-write-content \"blubb\" \"blubb\")"));
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
}

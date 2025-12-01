using Lisp;
using Lisp.Types;

namespace TestLisp;

[TestClass]
public sealed class TestBuiltin
{

    [TestMethod]
    [DataRow("(list)", "()")]
    [DataRow("(list? (list))", "true")]
    [DataRow("(list? nil)", "false")]
    [DataRow("(null? (list))", "true")]
    [DataRow("(null? (list 1))", "false")]
    [DataRow("(list 1 2 3)", "(1 2 3)")]
    [DataRow("(count (list 1 2 3))", "3")]
    [DataRow("(count (list))", "0")]
    public void Lists(string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("[]", "[]")]
    [DataRow("(null? [])", "true")]
    [DataRow("(null? [1])", "false")]
    [DataRow("(count [1 2 3])", "3")]
    [DataRow("(count [])", "0")]
    [DataRow("(list? [1 2 3])", "false")]
    [DataRow("(vec '(1 2 3))", "[1 2 3]")]
    [DataRow("(vector? [1 2 3])", "true")]
    [DataRow("(vec (list))", "[]")]
    [DataRow("(vec []))", "[]")]
    [DataRow("(vec [1 2 3]))", "[1 2 3]")]
    public void Vectors(string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(if true 7 8)", "7")]
    [DataRow("(if false 7 8)", "8")]
    [DataRow("(if false 7 false)", "false")]
    [DataRow("(if true (+ 1 7) (+ 1 8))", "8")]
    [DataRow("(if false (+ 1 7) (+ 1 8))", "9")]
    [DataRow("(if nil 7 8)", "8")]
    [DataRow("(if 0 7 8)", "7")]
    [DataRow("(if (list) 7 8)", "7")]
    [DataRow("(if (list 1 2 3) 7 8)", "7")]
    [DataRow("(if false (+ 1 7))", "nil")]
    [DataRow("(if nil 8)", "nil")]
    [DataRow("(if nil 8 7)", "7")]
    [DataRow("(if true (+ 1 7))", "8")]
    [DataRow("(if [] 7 8)", "7")]
    public void If(string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

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
    public void Conditionals(string input, string expected)
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

    [DataRow("(= (list) (list))", "true")]
    [DataRow("(= (list) ())", "true")]
    [DataRow("(= (list 1 2) (list 1 2))", "true")]
    [DataRow("(= (list 1) (list))", "false")]
    [DataRow("(= (list) (list 1))", "false")]
    [DataRow("(= 0 (list))", "false")]
    [DataRow("(= (list) 0)", "false")]
    [DataRow("(= (list nil) (list))", "false")]
    [DataRow("(= (list) nil)", "false")]

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
    [DataRow("(= [] (list))", "true")]
    [DataRow("(= [7 8] [7 8])", "true")]
    [DataRow("(= [:abc] [:abc])", "true")]
    [DataRow("(= (list 1 2) [1 2])", "true")]
    [DataRow("(= (list 1) [])", "false")]
    [DataRow("(= [] [1])", "false")]
    [DataRow("(= 0 [])", "false")]
    [DataRow("(= [] 0)", "false")]
    [DataRow("(= [] \"\")", "false")]
    [DataRow("(= \"\" [])", "false")]
    [DataRow("(= [(list)] (list []))", "true")]
    [DataRow("(= [1 2 (list 3 4 [5 6])] (list 1 2 [3 4 (list 5 6)]))", "true")]
    [DataRow("(= (atom 23) (atom 23))", "false")]
    [DataRow("(= @(atom 23) @(atom 23))", "true")]
    public void Equality(string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("((lambda (a b) (+ b a)) 3 4)", "7")]
    [DataRow("((lambda () 4) )", "4")]
    [DataRow("((lambda () ()) )", "()")]
    [DataRow("((lambda (f x) (f x)) (lambda (a) (+ 1 a)) 7)", "8")]
    [DataRow("(((lambda (a) (lambda (b) (+ a b))) 5) 7)", "12")]
    [DataRow("(define gen-plus5 (lambda () (lambda (b) (+ 5 b))))\n(define plus5 (gen-plus5))\n(plus5 7)", "12")]
    [DataRow("(define gen-plusX (lambda (x) (lambda (b) (+ x b))))\n(define plus7 (gen-plusX 7))\n(plus7 8)", "15")]
    [DataRow("(let [b 0 f (lambda [] b)] (let [b 1] (f)))", "0")]
    [DataRow("((let [b 0] (lambda [] b)))", "0")]
    [DataRow("((lambda [] 4) )", "4")]
    [DataRow("((lambda [f x] (f x)) (lambda [a] (+ 1 a)) 7)", "8")]
    public void Lambda(string input, string expected)
    {
        var sut = new LispEnvironment();
        var result = string.Empty;

        foreach (var i in input.Split('\n'))
            result = sut.ReadEvaluatePrint(i);

        Assert.AreEqual(expected, result, "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("((lambda (& more) (count more)) 1 2 3)", "3")]
    [DataRow("((lambda (& more) (list? more)) 1 2 3)", "true")]
    [DataRow("((lambda (& more) (count more)) 1)", "1")]
    [DataRow("((lambda (& more) (count more)) )", "0")]
    [DataRow("((lambda (& more) (list? more)) )", "true")]
    [DataRow("((lambda (a & more) (count more)) 1 2 3)", "2")]
    [DataRow("((lambda (a & more) (count more)) 1)", "0")]
    [DataRow("((lambda (a & more) (list? more)) 1)", "true")]

    public void LambdaVariadic(string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(do (+ 3 4))", "7")]
    [DataRow("(do (* 2 3) (+ 3 4))", "7")]
    [DataRow("(do (* 2 3) 7)", "7")]
    [DataRow("(do (define a 6) 7 (+ a 8))", "14")]
    [DataRow("(do (do 1 2))", "2")]
    public void Do(string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(not false)", "true")]
    [DataRow("(not nil)", "true")]
    [DataRow("(not true)", "false")]
    [DataRow("(not \"a\")", "false")]
    [DataRow("(not 0)", "false")]
    public void Not(string input, string expected)
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
    [DataRow("(print (list))", "\"()\"")]
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
    public void Strcat(string input, string expected)
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
    [DataRow("(type-of (list))", typeof(LispList))]
    [DataRow("(type-of (list 1 2 3))", typeof(LispList))]
    [DataRow("(type-of [])", typeof(LispVector))]
    [DataRow("(type-of {})", typeof(LispHashMap))]
    [DataRow("(type-of \"abc\")", typeof(LispString))]
    [DataRow("(type-of 'abc)", typeof(LispSymbol))]
    [DataRow("(type-of :abc)", typeof(LispKeyword))]
    [DataRow("(type-of (lambda (x) x))", typeof(LispFunction))]
    [DataRow("(type-of +)", typeof(LispPrimitive))]
    [DataRow("(type-of (atom 42))", typeof(LispAtom))]
    public void TypeOf(string input, Type expected)
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
    public void Swap()
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
    [DataRow("(cons 1 (list))", "(1)")]
    [DataRow("(cons 1 (list 2))", "(1 2)")]
    [DataRow("(cons 1 (list 2 3))", "(1 2 3)")]
    [DataRow("(cons (list 1) (list 2 3))", "((1) 2 3)")]
    [DataRow("(list (cons 1 a) a) ", "((1 2 3) (2 3))")]
    [DataRow("(cons 1 [])", "(1)")]
    [DataRow("(cons [1] [2 3])", "([1] 2 3)")]
    [DataRow("(cons 1 [2 3])", "(1 2 3)")]
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
    [DataRow("(concat [1 2] (list 3 4) (list 5 6))", "(1 2 3 4 5 6)")]
    [DataRow("(concat [1 2])", "(1 2)")]
    public void Concat(string input, string expected)
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
    //[DataRow("(first nil)", "nil")]
    [DataRow("(first [])", "nil")]
    [DataRow("(first [6])", "6")]
    [DataRow("(first [6 7 8])", "6")]
    public void First(string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(rest (list))", "()")]
    [DataRow("(rest (list 6))", "()")]
    [DataRow("(rest (list 6 7 8))", "(7 8)")]
    //[DataRow("(rest nil)", "nil")]
    [DataRow("(rest [])", "()")]
    [DataRow("(rest [6])", "()")]
    [DataRow("(rest [6 7 8])", "(7 8)")]
    public void Rest(string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }


    [TestMethod]
    [DataRow("(nth (list 1) 0)", "1")]
    [DataRow("(nth (list 1 2) 1)", "2")]
    [DataRow("(nth (list 1 2 nil) 2)", "nil")]
    [DataRow("(nth [1] 0)", "1")]
    [DataRow("(nth [1 2] 1)", "2")]
    [DataRow("(nth [1 2 nil] 2)", "nil")]
    public void Nth(string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(cond)", "nil")]
    [DataRow("(cond true 7)", "7")]
    [DataRow("(cond false 7)", "nil")]
    [DataRow("(cond true 7 true 8)", "7")]
    [DataRow("(cond false 7 true 8)", "8")]
    [DataRow("(cond false 7 false 8 \"else\" 9)", "9")]
    [DataRow("(cond false 7 (= 2 2) 8 \"else\" 9)", "8")]
    [DataRow("(cond false 7 false 8 false 9)", "nil")]
    [DataRow("(let (x (cond false \"no\" true \"yes\")) x)", "\"yes\"")]
    [DataRow("(let [x (cond false \"no\" true \"yes\")] x)", "\"yes\"")]
    public void Cond (string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
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
    public void Symbol(string input, string expected)
    {
        Assert.AreEqual(expected, new LispEnvironment().ReadEvaluatePrint(input), "input:<{0}>", input);
    }

    [TestMethod]
    [DataRow("(map double numbers)", "(2 4 6)")]
    [DataRow("(map (lambda (x) (symbol? x)) (list 1 (quote two) \"three\"))", "(false true false)")]
    [DataRow("(= () (map strcat ()))", "true")]
    [DataRow("(map (lambda (a) (* 2 a)) [1 2 3])", "(2 4 6)")]
    [DataRow("(map (lambda [& args] (list? args)) [1 2])", "(true true)")]
    public void Map (string input, string expected)
    {
        var sut = new LispEnvironment();
        sut.ReadEvaluatePrint("(define numbers (list 1 2 3))");
        sut.ReadEvaluatePrint("(define double (lambda (x) (* 2 x)))");
        Assert.AreEqual(expected, sut.ReadEvaluatePrint(input), "input:<{0}>", input);
    }


    [TestMethod]
    public void Quine ()
    {
        const string quine = "((lambda (q) (quasiquote ((unquote q) (quote (unquote q))))) (quote (lambda (q) (quasiquote ((unquote q) (quote (unquote q)))))))";
        Assert.AreEqual(quine, new LispEnvironment().ReadEvaluatePrint(quine));
    }
}

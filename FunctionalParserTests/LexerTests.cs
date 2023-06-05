using FunctionalParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static FunctionalParser.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionalParser.Tests
{
    [TestClass()]
    public class LexerTests
    {
        private ILexer Char(char c)
        {
            return (s) => string.IsNullOrEmpty(s) || c != s[0]
                ? new List<(IList<string>, string)> { }
                : new List<(IList<string>, string)> { (new List<string> { c.ToString() }, s[1..]) };
        }

        [TestMethod()]
        public void SequenceTest()
        {
            {
                var test = "";
                var lexer = Sequence(Char('a'));
                var results = lexer.Invoke(test);
                Assert.AreEqual(results.Count, 0);
            }
            {
                var test = "a";
                var lexer = Sequence(Char('a'));
                var results = lexer.Invoke(test);
                Assert.AreEqual(results.Count, 1);
                Assert.AreEqual(results[0].Item2, "");
                Assert.IsTrue(results[0].Item1.SequenceEqual(new List<string> { "a" }));
            }
            {
                var test = "abc";
                var lexer = Sequence(Char('a'), Char('b'), Char('c'));
                var results = lexer.Invoke(test);
                Assert.AreEqual(results.Count, 1);
                Assert.AreEqual(results[0].Item2, "");
                Assert.IsTrue(results[0].Item1.SequenceEqual(new List<string> { "a", "b", "c" }));
            }
            {
                var test = "a";
                var lexer = Sequence(Char('a'), Char('b'), Char('c'));
                var results = lexer.Invoke(test);
                Assert.AreEqual(results.Count, 0);
            }
            {
                var test = "abcdef";
                var lexer = Sequence(Char('a'), Char('b'), Char('c'));
                var results = lexer.Invoke(test);
                Assert.AreEqual(results.Count, 1);
                Assert.AreEqual(results[0].Item2, "def");
                Assert.IsTrue(results[0].Item1.SequenceEqual(new List<string> { "a", "b", "c" }));
            }
        }

        [TestMethod()]
        public void ChoiceTest()
        {
            {
                var test = "a";
                var lexer = Choice(Char('a'), Char('b'));
                var results = lexer.Invoke(test);
                Assert.AreEqual(results.Count, 1);
                Assert.AreEqual(results[0].Item2, "");
                Assert.IsTrue(results[0].Item1.SequenceEqual(new List<string> { "a" }));
            }
            {
                var test = "a";
                var lexer = Choice(Char('b'), Char('a'));
                var results = lexer.Invoke(test);
                Assert.AreEqual(results.Count, 1);
                Assert.AreEqual(results[0].Item2, "");
                Assert.IsTrue(results[0].Item1.SequenceEqual(new List<string> { "a" }));
            }
            {
                var test = "a";
                var lexer = Choice(Char('c'), Char('d'));
                var results = lexer.Invoke(test);
                Assert.AreEqual(results.Count, 0);
            }
            {
                var test = "";
                var lexer = Choice(Char('c'), Char('d'));
                var results = lexer.Invoke(test);
                Assert.AreEqual(results.Count, 0);
            }
        }

        [TestMethod()]
        public void SomeTest()
        {
            {
                var test = "a";
                var lexer = Some(Char('a'));
                var results = lexer.Invoke(test);
                Assert.AreEqual(1, results.Count);
                Assert.AreEqual("", results[0].Item2);
                Assert.IsTrue(results[0].Item1.SequenceEqual(new List<string> { "a" }));
            }
            {
                var test = "aaa";
                var lexer = Some(Char('a'));
                var results = lexer.Invoke(test);
                Assert.AreEqual(1, results.Count);
                Assert.AreEqual("", results[0].Item2);
                Assert.IsTrue(results[0].Item1.SequenceEqual(new List<string> { "a", "a", "a" }));
            }
            {
                var test = "aabbaa";
                var lexer = Some(Char('a'));
                var results = lexer.Invoke(test);
                Assert.AreEqual(1, results.Count);
                Assert.IsTrue(results[0].Item1.SequenceEqual(new List<string> { "a", "a" }));
            }
            {
                var test = "abbaa";
                var lexer = Some(Char('b'));
                var results = lexer.Invoke(test);
                Assert.AreEqual(0, results.Count);
            }
        }
    }
}
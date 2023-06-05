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
                ? (new List<string>(), s, false)
                : (new List<string> { c.ToString() }, s[1..], true);
        }

        [TestMethod()]
        public void SequenceTest()
        {
            {
                var test = "";
                var lexer = Sequence(Char('a'));
                var (result, remaining, isSuccess) = lexer.Invoke(test);
                Assert.IsFalse(isSuccess);
            }
            {
                var test = "a";
                var lexer = Sequence(Char('a'));
                var (result, remaining, isSuccess) = lexer.Invoke(test);
                Assert.IsTrue(isSuccess);
                Assert.AreEqual(remaining, "");
                Assert.IsTrue(result.SequenceEqual(new List<string> { "a" }));
            }
            {
                var test = "abc";
                var lexer = Sequence(Char('a'), Char('b'), Char('c'));
                var (result, remaining, isSuccess) = lexer.Invoke(test);
                Assert.IsTrue(isSuccess);
                Assert.AreEqual(remaining, "");
                Assert.IsTrue(result.SequenceEqual(new List<string> { "a", "b", "c" }));
            }
            {
                var test = "a";
                var lexer = Sequence(Char('a'), Char('b'), Char('c'));
                var (result, remaining, isSuccess) = lexer.Invoke(test);
                Assert.IsFalse(isSuccess);
                Assert.AreEqual(remaining, "a");
            }
            {
                var test = "abcdef";
                var lexer = Sequence(Char('a'), Char('b'), Char('c'));
                var (result, remaining, isSuccess) = lexer.Invoke(test);
                Assert.IsTrue(isSuccess);
                Assert.AreEqual(remaining, "def");
                Assert.IsTrue(result.SequenceEqual(new List<string> { "a", "b", "c" }));
            }
        }

        [TestMethod()]
        public void ChoiceTest()
        {
            {
                var test = "";
                var lexer = Choice(Char('a'), Char('b'));
                var (result, remaining, isSuccess) = lexer.Invoke(test);
                Assert.IsFalse(isSuccess);
            }
            {
                var test = "a";
                var lexer = Choice(Char('a'), Char('b'));
                var (result, remaining, isSuccess) = lexer.Invoke(test);
                Assert.IsTrue(isSuccess);
                Assert.AreEqual(remaining, "");
                Assert.IsTrue(result.SequenceEqual(new List<string> { "a" }));
            }
            {
                var test = "a";
                var lexer = Choice(Char('b'), Char('a'));
                var (result, remaining, isSuccess) = lexer.Invoke(test);
                Assert.IsTrue(isSuccess);
                Assert.AreEqual(remaining, "");
                Assert.IsTrue(result.SequenceEqual(new List<string> { "a" }));
            }
            {
                var test = "a";
                var lexer = Choice(Char('c'), Char('d'));
                var (result, remaining, isSuccess) = lexer.Invoke(test);
                Assert.IsFalse(isSuccess);
                Assert.AreEqual(remaining, "a");
            }
        }

        [TestMethod()]
        public void SomeTest()
        {
            {
                var test = "";
                var lexer = Some(Char('a'));
                var (result, remaining, isSuccess) = lexer.Invoke(test);
                Assert.IsFalse(isSuccess);
            }
            {
                var test = "a";
                var lexer = Some(Char('a'));
                var (result, remaining, isSuccess) = lexer.Invoke(test);
                Assert.IsTrue(isSuccess);
                Assert.AreEqual(remaining, "");
                Assert.IsTrue(result.SequenceEqual(new List<string> { "a" }));
            }
            {
                var test = "aa";
                var lexer = Some(Char('a'));
                var (result, remaining, isSuccess) = lexer.Invoke(test);
                Assert.IsTrue(isSuccess);
                Assert.AreEqual(remaining, "");
                Assert.IsTrue(result.SequenceEqual(new List<string> { "a", "a" }));
            }
            {
                var test = "aaa";
                var lexer = Some(Char('a'));
                var (result, remaining, isSuccess) = lexer.Invoke(test);
                Assert.IsTrue(isSuccess);
                Assert.AreEqual(remaining, "");
                Assert.IsTrue(result.SequenceEqual(new List<string> { "a", "a", "a" }));
            }
            {
                var test = "abbaa";
                var lexer = Some(Char('a'));
                var (result, remaining, isSuccess) = lexer.Invoke(test);
                Assert.IsTrue(isSuccess);
                Assert.AreEqual(remaining, "bbaa");
                Assert.IsTrue(result.SequenceEqual(new List<string> { "a", }));
            }
            {
                var test = "aabbaa";
                var lexer = Some(Char('a'));
                var (result, remaining, isSuccess) = lexer.Invoke(test);
                Assert.IsTrue(isSuccess);
                Assert.AreEqual(remaining, "bbaa");
                Assert.IsTrue(result.SequenceEqual(new List<string> { "a", "a" }));
            }
            {
                var test = "abbaa";
                var lexer = Some(Char('b'));
                var (result, remaining, isSuccess) = lexer.Invoke(test);
                Assert.IsFalse(isSuccess);
                Assert.AreEqual(remaining, "abbaa");
            }
        }
    }
}

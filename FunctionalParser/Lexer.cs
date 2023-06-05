using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionalParser
{
    public delegate (IList<string>, string, bool) ILexer(string stream);

    // x*   => Choice(Some(x), Nothing)
    // x+   => Some(x)
    // x|y  => Choice(x, y)
    // x,y  => Sequence(x, y)
    public static class Lexer
    {
        private static (IList<string>, string, bool) Fail(string s)
        {
            return (Array.Empty<string>(), s, false);
        }

        public static readonly ILexer LETTER_LOWERCASE = Range('a', 'z');
        public static readonly ILexer LETTER_UPPERCASE = Range('A', 'Z');
        public static readonly ILexer LETTER = Choice(LETTER_LOWERCASE, LETTER_UPPERCASE);
        public static readonly ILexer DIGIT = Range('0', '9');
        public static readonly ILexer NZ_DIGIT = Range('1', '9');

        public static ILexer Nothing()
        {
            return (s) => (Array.Empty<string>(), s, true);
        }

        /// <summary>
        /// Returns a new ILexer that sequentially tries the given lexers,
        /// returning success on the first match, and failure if all lexers
        /// fail.
        ///
        /// This is analagous to the regex alternation operator i.e. x|y.
        /// </summary>
        /// <param name="lexers"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If lexers is empty</exception>
        public static ILexer Choice(params ILexer[] lexers)
        {
            if (lexers.Length == 0)
            {
                throw new ArgumentException("Required one or more lexers");
            }
            return (s) =>
            {
                foreach (ILexer lx in lexers)
                {
                    var (result, remaining, isSuccess) = lx.Invoke(s);
                    if (isSuccess)
                    {
                        return (result, remaining, isSuccess);
                    }
                }
                return Fail(s);
            };
        }

        /// <summary>
        /// Returns a new ILexer that attempts to use the given lexer one or
        /// more times, returning success if at least one successful use is
        /// performed, and failure otherwise.
        /// 
        /// This is analagous to the regex one-or-more operator i.e. x+
        /// </summary>
        /// <param name="lexer"></param>
        /// <returns></returns>
        public static ILexer Some(ILexer lexer)
        {
            return (s) =>
            {
                var (result, remaining, isSuccess) = lexer.Invoke(s);
                if (!isSuccess)
                {
                    return Fail(s);
                }
                var builder = new List<IList<string>>();
                while (isSuccess)
                {
                    builder.Add(result);
                    (result, remaining, isSuccess) = lexer.Invoke(remaining);
                }
                return (
                    builder.Select(x => x.Aggregate((x, y) => x + y)).ToList(),
                    remaining,
                    true
                );
            };
        }

        /// <summary>
        /// Returns a new ILexer that successive uses the given lexers one
        /// after the other, returning success if all lexers have been
        /// successfully used, and failure otherwise.
        /// 
        /// This is analagous to regex concatenation.
        /// </summary>
        /// <param name="lexers"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static ILexer Sequence(params ILexer[] lexers)
        {
            if (lexers.Length == 0)
            {
                throw new ArgumentException("Required one or more lexers");
            }
            return (s) =>
            {
                var (result, remaining, isSuccess) = Fail("");
                remaining = s;
                var builder = new List<IList<string>>();
                foreach (ILexer lx in lexers)
                {
                    (result, remaining, isSuccess) = lx.Invoke(remaining);
                    builder.Add(result);
                    if (!isSuccess)
                    {
                        return Fail(s);
                    }
                }
                return (
                    builder.Select(x => x.Aggregate("", (x, y) => x + y)).ToList(),
                    remaining,
                    true
                );
            };
        }

        /// <summary>
        /// Returns a new ILexer that matches a single character.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static ILexer Char(char c)
        {
            return Range(c, c);
        }

        /// <summary>
        /// Returns a new ILexer that matches a range of characters given by
        /// the expression (a &lt;= S[0] &lt;= b), where S is the input string.
        /// For example ('a' &lt;= 'm' &lt;= 'b') but not ('a' &lt;= '|' &lt;=
        /// 'z').
        /// 
        /// This is analagous to the regex shorthands a-z, A-Z, 1-9, etc.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static ILexer Range(char a, char b)
        {
            return (s) =>
            {
                if (string.IsNullOrEmpty(s))
                {
                    return Fail(s);
                }
                return a <= s[0] && s[0] <= b
                    ? (new List<string> { s[0].ToString() }, s[1..], true)
                    : Fail(s);
            };
        }
     
        /// <summary>
        /// Returns a new ILexer that matches a string. Note that this is
        /// different from Sequence(Char(c1), Char(c2), ..., Char(cn)). This
        /// Lexer will return a single element list containing the string t on
        /// success, whereas the sequence will return a list of
        /// single-character strings instead.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static ILexer CharSequence(string t)
        {
            return (s) => s.StartsWith(t) ? (new List<string> { t }, s[t.Length..], true) : Fail(s);
        }

        /// <summary>
        /// Returns a new ILexer that uses the given lexer zero or more times.
        /// Note that this means that this lexer cannot fail. This is a
        /// convenience method and is equivalent to Choice(Some(lexer),
        /// Nothing()).
        ///
        /// This is analagous to the regex kleene star operator i.e. x*.
        /// </summary>
        /// <param name="lexer"></param>
        /// <returns></returns>
        public static ILexer ZeroOrMore(ILexer lexer)
        {
            return Choice(Some(lexer), Nothing());
        }

        /// <summary>
        /// Returns a new ILexer that returns success if the input string is
        /// successfully lexed by the given lexer, but does not consume any of
        /// the input string.
        /// 
        /// Chaining this method is not supported.
        /// 
        /// This is analagous to regex positive lookahead i.e. x(?=y).
        /// </summary>
        /// <param name="lexer"></param>
        /// <returns></returns>
        public static ILexer AssertAfter(ILexer lexer)
        {
            return (s) =>
            {
                var (result, remaining, isSuccess) = lexer.Invoke(s);
                return isSuccess ? (Array.Empty<string>(), s, true) : Fail(s);
            };
        }

        /// <summary>
        /// Returns a new ILexer that returns success if the input string
        /// cannot be successfully lexed by the given lexed, and does not
        /// consume any of the input string.
        /// 
        /// Chaining this method is not supported.
        /// 
        /// This is analagous to regex negative lookahead i.e. x(?!y).
        /// </summary>
        /// <param name="lexer"></param>
        /// <returns></returns>
        public static ILexer AssertNotAfter(ILexer lexer)
        {
            return (s) =>
            {
                var (result, remaining, isSuccess) = lexer.Invoke(s);
                return isSuccess ? Fail(s) : (Array.Empty<string>(), s, true);
            };
        }
    }
}

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

        public static ILexer Choice(params ILexer[] lexers)
        {
            if (lexers.Length == 0)
            {
                throw new ArgumentException("Required one or more lexers");
            }
            return (s) => lexers
                .Select(lx => lx.Invoke(s))
                .FirstOrDefault(result => result.Item3, Fail(s));
        }

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

        public static ILexer Sequence(params ILexer[] lexers)
        {
            if (lexers.Length == 0)
            {
                throw new ArgumentException("Required one or more lexers");
            }
            return (s) =>
            {
                var (result, remaining, isSuccess) = Fail(s); //placeholder
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

        public static ILexer Char(char c)
        {
            return Range(c, c);
        }

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

        public static ILexer CharSequence(string t)
        {
            return (s) => s.StartsWith(t) ? (new List<string> { t }, s[t.Length..], true) : Fail(s);
        }

        public static ILexer ZeroOrMore(ILexer lexer)
        {
            return Choice(Some(lexer), Nothing());
        }

        public static ILexer AssertAfter(ILexer lexer)
        {
            return (s) =>
            {
                var (result, remaining, isSuccess) = lexer.Invoke(s);
                return isSuccess ? (Array.Empty<string>(), s, true) : Fail(s);
            };
        }

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

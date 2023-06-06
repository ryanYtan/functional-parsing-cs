// x*   => Choice(Some(x), Nothing)
// x+   => Some(x)
// x|y  => Choice(x, y)
// x,y  => Sequence(x, y)

namespace FunctionalParser
{
    using static Lexer;
    using LexerResult = Result<IEnumerable<string>, string>;

    public delegate LexerResult ILexer(string stream);

    public static class Lexer
    {
        public static readonly ILexer LETTER_LOWERCASE = Range('a', 'z');
        public static readonly ILexer LETTER_UPPERCASE = Range('A', 'Z');
        public static readonly ILexer LETTER = Choice(LETTER_LOWERCASE, LETTER_UPPERCASE);
        public static readonly ILexer DIGIT = Range('0', '9');
        public static readonly ILexer NZ_DIGIT = Range('1', '9');
    
        public static ILexer Nothing()
        {
            return (s) => LexerResult.Success(Array.Empty<string>(), s);
        }
    
        public static ILexer Choice(params ILexer[] lexers)
        {
            if (lexers.Length <= 0)
            {
                throw new ArgumentException("Required one or more lexers");
            }
            return (s) => lexers
                .Select(lx => lx.Invoke(s))
                .FirstOrDefault(lex => lex.IsSuccess, LexerResult.Fail(s));
        }
    
        public static ILexer Some(ILexer lexer)
        {
            return (s) =>
            {
                var (value, remaining, isSuccess) = lexer.Invoke(s);
                if (!isSuccess)
                {
                    return LexerResult.Fail(s);
                }
                var builder = new List<IEnumerable<string>>();
                while (isSuccess)
                {
                    builder.Add(value.Get());
                    (value, remaining, isSuccess) = lexer.Invoke(remaining);
                }
                return LexerResult.Success(
                    builder.Select(x => x.Aggregate((x, y) => x + y)).AsEnumerable(),
                    remaining
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
                var (value, remaining, isSuccess) = LexerResult.Fail(s); //placeholder
                var builder = new List<IEnumerable<string>>();
                foreach (ILexer lx in lexers)
                {
                    (value, remaining, isSuccess) = lx.Invoke(remaining);
                    if (!isSuccess)
                    {
                        return LexerResult.Fail(s);
                    }
                    builder.Add(value.Get());
                }
                return LexerResult.Success(
                    builder.Select(x => x.Aggregate("", (x, y) => x + y)).ToList(),
                    remaining
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
                return string.IsNullOrEmpty(s) || s[0] < a || b < s[0]
                    ? LexerResult.Fail(s)
                    : LexerResult.Success(new List<string> { s[0].ToString() }, s[1..]);
            };
        }
    
        public static ILexer CharSequence(string t)
        {
            return (s) => s.StartsWith(t)
                ? LexerResult.Success(new List<string> { t }, s[t.Length..])
                : LexerResult.Fail(s);
        }
    
        public static ILexer ZeroOrMore(ILexer lexer)
        {
            return Choice(Some(lexer), Nothing());
        }
    
        public static ILexer FullMatch(ILexer lexer)
        {
            return (s) =>
            {
                var (value, remaining, isSuccess) = lexer.Invoke(s);
                return remaining.Length == 0
                    ? LexerResult.Success(value.Get(), "")
                    : LexerResult.Fail(s);
            };
        }
    
        public static ILexer AssertAfter(ILexer lexer)
        {
            return (s) =>
            {
                var (value, remaining, isSuccess) = lexer.Invoke(s);
                return isSuccess ? LexerResult.Success(Array.Empty<string>(), s) : LexerResult.Fail(s);
            };
        }
    
        public static ILexer AssertNotAfter(ILexer lexer)
        {
            return (s) =>
            {
                var (value, remaining, isSuccess) = lexer.Invoke(s);
                return isSuccess ? LexerResult.Fail(s) : LexerResult.Success(Array.Empty<string>(), s);
            };
        }
    }

    public static class ProgramLexer
    {
        public static ILexer Program()
        {
            // Lexer definitions
            var skipSpaces = Some(Choice(Char(' '), Char('\n'), Char('\b'), Char('\t'), Char('\r')));
            var name = Sequence(
                LETTER,
                ZeroOrMore(Choice(LETTER, DIGIT))
            );
            var number = Choice(
                Sequence(Char('0'), AssertNotAfter(Choice(LETTER, DIGIT))),
                Sequence(NZ_DIGIT, ZeroOrMore(DIGIT))
            );
            var doubleCharacterOperators = Choice(
                new string[] { "!=", "==", "<=", ">=", "&&", "||" }.Select(x => CharSequence(x)).ToArray()
            );
            var singleCharacterOperators = Choice(
                "&|^~!=<>(){};+-*/%\"".Select(x => Char(x)).ToArray()
            );
            var program = FullMatch(ZeroOrMore(Choice(
                name,
                number,
                doubleCharacterOperators,
                singleCharacterOperators,
                skipSpaces
            )));
            return program;
        }
    }
}

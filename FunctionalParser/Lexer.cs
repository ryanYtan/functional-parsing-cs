
// x*   => Choice(Some(x), Nothing)
// x+   => Some(x)
// x|y  => Choice(x, y)
// x,y  => Sequence(x, y)

namespace FunctionalParser
{
    public delegate LexerResult ILexer(string stream);

    public class LexerResult
    {
        public IEnumerable<string> Result { get; set; }
        public string Remaining { get; set; }
        public bool IsSuccess { get; set; }
    
        public LexerResult(IEnumerable<string> result, string remaining, bool isSuccess)
        {
            Result = result;
            Remaining = remaining;
            IsSuccess = isSuccess;
        }
    
        public static LexerResult Success(IEnumerable<string> result, string remaining)
        {
            return new LexerResult(result, remaining, true);
        }
    
        public static LexerResult Fail(string remaining)
        {
            return new LexerResult(Array.Empty<string>(), remaining, false);
        }
    
        public void Deconstruct(out IEnumerable<string> result, out string remaining, out bool isSuccess)
        {
            result = Result;
            remaining = Remaining;
            isSuccess = IsSuccess;
        }
    }
    
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
                .FirstOrDefault(result => result.IsSuccess, LexerResult.Fail(s));
        }
    
        public static ILexer Some(ILexer lexer)
        {
            return (s) =>
            {
                var (result, remaining, isSuccess) = lexer.Invoke(s);
                if (!isSuccess)
                {
                    return LexerResult.Fail(s);
                }
                var builder = new List<IEnumerable<string>>();
                while (isSuccess)
                {
                    builder.Add(result);
                    (result, remaining, isSuccess) = lexer.Invoke(remaining);
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
                var (result, remaining, isSuccess) = LexerResult.Fail(s); //placeholder
                var builder = new List<IEnumerable<string>>();
                foreach (ILexer lx in lexers)
                {
                    (result, remaining, isSuccess) = lx.Invoke(remaining);
                    builder.Add(result);
                    if (!isSuccess)
                    {
                        return LexerResult.Fail(s);
                    }
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
                var result = lexer.Invoke(s);
                return result.Remaining.Length == 0
                    ? LexerResult.Success(result.Result, "")
                    : LexerResult.Fail(s);
            };
        }
    
        public static ILexer AssertAfter(ILexer lexer)
        {
            return (s) =>
            {
                var (result, remaining, isSuccess) = lexer.Invoke(s);
                return isSuccess ? LexerResult.Success(Array.Empty<string>(), s) : LexerResult.Fail(s);
            };
        }
    
        public static ILexer AssertNotAfter(ILexer lexer)
        {
            return (s) =>
            {
                var (result, remaining, isSuccess) = lexer.Invoke(s);
                return isSuccess ? LexerResult.Fail(s) : LexerResult.Success(Array.Empty<string>(), s);
            };
        }
    }
}

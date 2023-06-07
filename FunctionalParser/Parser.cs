using static FunctionalParser.Parser;

namespace FunctionalParser
{
    public class ParserResult<T> : Result<T, IList<Token>>
    {
        protected ParserResult(Option<T> value, IList<Token> remaining, bool isSuccess)
            : base(value, remaining, isSuccess)
        { }

        public static new ParserResult<T> Success(T value, IList<Token> remaining)
        {
            return new ParserResult<T>(Option<T>.Some(value), remaining, true);
        }

        public static new ParserResult<T> Empty(IList<Token> remaining)
        {
            return new ParserResult<T>(Option<T>.None(), remaining, true);
        }

        public static new ParserResult<T> Fail(IList<Token> remaining)
        {
            return new ParserResult<T>(Option<T>.None(), remaining, false);
        }
    }

	public delegate ParserResult<T> IParser<T>(IList<Token> tokens);

	public static class Parser
	{
        public static IParser<SyntaxTree> Nothing()
        {
            return (tokens) => ParserResult<SyntaxTree>.Empty(tokens);
        }

        public static IParser<T> Choice<T>(params IParser<T>[] parsers)
        {
            if (parsers.Length == 0)
            {
                throw new ArgumentException("Required one or more parsers");
            }
            return (tokens) => parsers
                .Select(p => p.Invoke(tokens))
                .FirstOrDefault(p => p.IsSuccess, ParserResult<T>.Fail(tokens));
        }

        public static IParser<T> Sequence<T>(params IParser<T>[] parsers)
        {
            if (parsers.Length == 0)
            {
                throw new ArgumentException("Required one or more parsers");
            }
            return (tokens) =>
            {
                foreach (var parser in parsers)
                { 
                }
            };
        }

        public static IParser<SyntaxTree> ExpectString<SyntaxTree>(string t)
        {
            return (tokens) =>
            {
                if (tokens.Count == 0 || tokens[0].CharSeq != t)
                {
                    return ParserResult<SyntaxTree>.Fail(tokens);
                }
                return ParserResult<SyntaxTree>.Empty(tokens.Skip(1).ToList());
            };
        }

        public static IParser<SyntaxTree> ExpectType<SyntaxTree>(TokenType type)
        {
            return (tokens) =>
            {
                if (tokens.Count == 0)
                {
                    return ParserResult<SyntaxTree>.Fail(tokens);
                }
                return tokens[0].Type == type
                    ? ParserResult<SyntaxTree>.Empty(tokens.Skip(1).ToList())
                    : ParserResult<SyntaxTree>.Fail(tokens);
            };
        }
	}

    public static class ProgramParser
    {
        public static IParser<SyntaxTree> Program()
        {
            return Nothing();
        }
    }
}


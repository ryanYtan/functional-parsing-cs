using static FunctionalParser.Parser;

namespace FunctionalParser
{
    using ParserResult = Result<SyntaxTree, IList<Token>>;
	public delegate ParserResult IParser(IList<Token> tokens);

	public static class Parser
	{
        public static IParser Nothing()
        {
            return (tokens) => ParserResult.Empty(tokens);
        }

        public static IParser Choice(params IParser[] parsers)
        {
            if (parsers.Length == 0)
            {
                throw new ArgumentException("Required one or more parsers");
            }
            return (tokens) => parsers
                .Select(p => p.Invoke(tokens))
                .FirstOrDefault(p => p.IsSuccess, ParserResult.Fail(tokens));
        }

        public static IParser Sequence(params IParser[] parsers)
        {
            if (parsers.Length == 0)
            {
                throw new ArgumentException("Required one or more parsers");
            }
            return (tokens) =>
            {

            };
        }

        public static IParser ExpectString(string t)
        {
            return (tokens) =>
            {
                if (tokens.Count == 0 || tokens[0].CharSeq != t)
                {
                    return ParserResult.Fail(tokens);
                }
                return ParserResult.Empty(tokens.Skip(1).ToList());
            };
        }

        public static IParser ExpectType(TokenType type)
        {
            return (tokens) =>
            {
                if (tokens.Count == 0)
                {
                    return ParserResult.Fail(tokens);
                }
                return tokens[0].Type == type
                    ? ParserResult.Empty(tokens.Skip(1).ToList())
                    : ParserResult.Fail(tokens);
            };
        }
	}

    public static class ProgramParser
    {
        public static IParser Program()
        {
            return Nothing();
        }
    }
}


using System;

namespace FunctionalParser
{
    using ParserResult = Result<IEnumerable<string>, string>;
	public delegate Result<T, IEnumerable<Token>> IParser<T>(IEnumerable<Token> tokens);

	public static class Parser
	{
	}
}


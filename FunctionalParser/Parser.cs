using System;

namespace FunctionalParser
{
	public delegate Result<T, IEnumerable<Token>> IParser<T>(IEnumerable<Token> tokens);

	public static class Parser
	{
	}
}


using FunctionalParser;
using System.Text;
using static FunctionalParser.Lexer;

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


//read program source from stdin
var sb = new StringBuilder();
int c = ' ';
while ((c = Console.Read()) != -1)
{
    sb.Append((char)c);
}
var programSource = sb.ToString();

//parse program
program.Invoke(programSource)
    .Result
    .Where(x => !string.IsNullOrWhiteSpace(x))
    .Select(x => Token.From(x))
    .ToList()
    .ForEach(x => Console.WriteLine(x));

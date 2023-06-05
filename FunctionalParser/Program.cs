using System.Text;
using static FunctionalParser.Lexer;

var sb = new StringBuilder();
int c = ' ';
while ((c = Console.Read()) != -1)
{
    sb.Append((char)c);
}
var programSource = sb.ToString();


var skipSpaces = Some(Char(' '));
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
    "!=<>(){};+-*/%".Select(x => Char(x)).ToArray()
);
var program = ZeroOrMore(Choice(
    name,
    number,
    doubleCharacterOperators,
    singleCharacterOperators,
    skipSpaces
));

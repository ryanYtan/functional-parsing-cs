using FunctionalParser;
using System.Text;
using static FunctionalParser.Lexer;

//read program source from stdin
var sb = new StringBuilder();
int c = ' ';
while ((c = Console.Read()) != -1)
{
    sb.Append((char)c);
}
var programSource = sb.ToString();

//parse program
ProgramLexer.Program()
    .Invoke(programSource)
    .Value
    .SelectMany(x => x.ToList())
    .Where(x => !string.IsNullOrWhiteSpace(x))
    .Select(x => Token.From(x))
    .ToList()
    .ForEach(x => Console.WriteLine(x));

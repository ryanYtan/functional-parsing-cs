﻿using static FunctionalParser.Lexer;

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

var program = ZeroOrMore(Choice(name, number, doubleCharacterOperators, singleCharacterOperators, skipSpaces));

//var sourceCode = "name NS11000D prog 1 0 10 0 11";
var sourceCode = "name NS11000D";
//var sourceCode = "proc isPrime() { until = 0; call sqrt; i = 2; while (i <= until) { if (n % i == 0) then {     call end; } else { aa80Sx = 0; } } } procedure main { call isPrime; }";
var lexes = program.Invoke(sourceCode);



//PRINTING RESULTS
Console.WriteLine("---");
foreach (var s in lexes)
{
    Console.WriteLine($"result=\"{s.Item1.Aggregate("", (x, y) => x + ", " + y)}\", remaining=\"{s.Item2}\"");
}
Console.WriteLine("---");

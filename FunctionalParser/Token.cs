using BiMap;

namespace FunctionalParser
{
    public enum TokenType
    {
        KeywordFunction,
        KeywordCall,
        KeywordWhile,
        KeywordIf,
        KeywordLet,
        KeywordSkip,
        ArithmeticAdd,
        ArithmeticSubtract,
        ArithmeticMultiply,
        ArithmeticDivide,
        ArithmeticModulo,
        LogicalAnd,
        LogicalOr,
        LogicalLessThan,
        LogicalLessThanOrEqual,
        LogicalGreaterThan,
        LogicalGreaterThanOrEqual,
        LogicalEqual,
        LogicalNotEqual,
        ControlLeftBrace,
        ControlRightBrace,
        ControlLeftParenthesis,
        ControlRightParenthesis,
        ControlSeparator,
        ControlAssign,
        ControlComma,
        ControlQuote,
        GenericName,
        GenericNumber,
    }

    public class Token
    {
        public Token(TokenType type, string charSeq)
        {
            Type = type;
            CharSeq = charSeq;
        }

        public TokenType Type { get; init; }
        public string CharSeq { get; init; }

        public readonly static IBiMap<TokenType, string> TokenMap = new Func<BiMap<TokenType, string>>(() =>
        {
            var biMap = new BiMap<TokenType, string>();
            biMap.AddPair(TokenType.KeywordFunction, "function");
            biMap.AddPair(TokenType.KeywordCall, "call");
            biMap.AddPair(TokenType.KeywordWhile, "while");
            biMap.AddPair(TokenType.KeywordIf, "if");
            biMap.AddPair(TokenType.KeywordLet, "let");
            biMap.AddPair(TokenType.KeywordSkip, "skip");
            biMap.AddPair(TokenType.ArithmeticAdd, "+");
            biMap.AddPair(TokenType.ArithmeticSubtract, "-");
            biMap.AddPair(TokenType.ArithmeticMultiply, "*");
            biMap.AddPair(TokenType.ArithmeticDivide, "/");
            biMap.AddPair(TokenType.ArithmeticModulo, "%");
            biMap.AddPair(TokenType.LogicalAnd, "&");
            biMap.AddPair(TokenType.LogicalOr, "|");
            biMap.AddPair(TokenType.LogicalLessThan, "<");
            biMap.AddPair(TokenType.LogicalLessThanOrEqual, "<=");
            biMap.AddPair(TokenType.LogicalGreaterThan, ">");
            biMap.AddPair(TokenType.LogicalGreaterThanOrEqual, ">=");
            biMap.AddPair(TokenType.LogicalEqual, "==");
            biMap.AddPair(TokenType.LogicalNotEqual, "!=");
            biMap.AddPair(TokenType.ControlLeftBrace, "{");
            biMap.AddPair(TokenType.ControlRightBrace, "}");
            biMap.AddPair(TokenType.ControlLeftParenthesis, "(");
            biMap.AddPair(TokenType.ControlRightParenthesis, ")");
            biMap.AddPair(TokenType.ControlSeparator, ";");
            biMap.AddPair(TokenType.ControlAssign, "=");
            biMap.AddPair(TokenType.ControlComma, ",");
            biMap.AddPair(TokenType.ControlQuote, "\"");
            return biMap;
        }).Invoke();

        public static Token From(string s)
        {
            var isNumber = (string t) => t.All(char.IsDigit);
            var isName = (string t) => char.IsLetter(t[0]) && t[1..].All(char.IsLetterOrDigit);

            var type = s switch
            {
                _ when TokenMap.TryGetRightToLeft(s, out var tokenType) => tokenType,
                _ when isNumber(s) => TokenType.GenericNumber,
                _ when isName(s) => TokenType.GenericName,
                _ => throw new ArgumentException($"Unknown token '{s}'"),
            };
            return new Token(type, s);
        }

        public override string ToString() => $"Token(\"{CharSeq}\", {Type})";
    }
}

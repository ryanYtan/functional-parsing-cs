using System;
namespace FunctionalParser
{
    public abstract class SyntaxTree
    {
    }

    public abstract class Program : SyntaxTree
    {
        public IEnumerable<Function> Functions { get; set; }

        public Program(IEnumerable<Function> functions)
        {
            Functions = functions;
        }
    }

    public abstract class Function : SyntaxTree
    {
        public FunctionDeclaration Definition { get; set; }
        public IEnumerable<Statement> Statement { get; set; }

        public Function(FunctionDeclaration definition, IEnumerable<Statement> statement)
        {
            Definition = definition;
            Statement = statement;
        }
    }

    public class FunctionDeclaration : SyntaxTree
    {
        public Token Name { get; set; }
        public IEnumerable<Token> Arguments { get; set; }

        public FunctionDeclaration(Token name, IEnumerable<Token> arguments)
        {
            Name = name;
            Arguments = arguments;
        }
    }

    public abstract class Statement : SyntaxTree
    { 
    }

    public class Assign : Statement
    {
        public Variable Assignee { get; set; }
        public Expression Expr { get; set; }

        public Assign(Variable assignee, Expression expr)
        {
            Assignee = assignee;
            Expr = expr;
        }
    }

    public class If : Statement
    {
        public Expression Condition { get; set; }
        public IEnumerable<Statement> IfStatements { get; set; }
        public IEnumerable<Statement> ElseStatements { get; set; }

        public If(Expression condition, IEnumerable<Statement> ifStatements, IEnumerable<Statement> elseStatements)
        {
            Condition = condition;
            IfStatements = ifStatements;
            ElseStatements = elseStatements;
        }
    }

    public class While : Statement
    {
        public Expression Condition { get; set; }
        public IEnumerable<Statement> Statements { get; set; }

        public While(Expression condition, IEnumerable<Statement> statements)
        {
            Condition = condition;
            Statements = statements;
        }
    }

    public class Call : Statement
    {
        public FunctionDeclaration Function { get; set; }

        public Call(FunctionDeclaration function)
        {
            Function = function;
        }
    }

    public class Return : Statement
    {
        public Expression Expr { get; set; }

        public Return(Expression expr)
        {
            Expr = expr;
        }
    }

    public class Skip : Statement
    { 
    }

    public abstract class Expression : SyntaxTree
    { 
    }

    public abstract class Value : Expression
    { 
    }

    public class FunctionExpression : Value
    {
        public FunctionDeclaration Callee { get; set; }

        public FunctionExpression(FunctionDeclaration callee)
        {
            Callee = callee;
        }
    }

    public class Variable : Value
    {
        public Token Name { get; set; }

        public Variable(Token name)
        {
            Name = name;
        }
    }

    public class Integer : Value
    {
        public Token Number { get; set; }

        public Integer(Token number)
        {
            Number = number;
        }
    }

    public class BinaryOperator : Expression
    {
        public Token Operator { get; set; }
        public Expression LeftHandSide { get; set; }
        public Expression RightHandSide { get; set; }

        public BinaryOperator(Token @operator, Expression leftHandSide, Expression rightHandSide)
        {
            Operator = @operator;
            LeftHandSide = leftHandSide;
            RightHandSide = rightHandSide;
        }
    }

    public class UnaryOperator : Expression
    {
        public Token Operator { get; set; }
        public Expression Expr { get; set; }

        public UnaryOperator(Token @operator, Expression expr)
        {
            Operator = @operator;
            Expr = expr;
        }
    }
}

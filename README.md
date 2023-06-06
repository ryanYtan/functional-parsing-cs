# CSharp Functional Parsing
An experiment in combinator parsing in .NET C#

## Syntax Grammar
```ebnf
LETTER          = [A-Z] | [a-z] ;
DIGIT           = [0-9] ;
NZ_DIGIT        = [1-9] ;
NAME            = (LETTER | "_") (LETTER | DIGIT | "_")* ;
INTEGER         = "0" | NZDIGIT (DIGIT)* ;

args_lst        = ""
                | NAME
                | NAME "," args_lst ;

function_dec    = NAME "(" args_lst ")"

program         = function+ ;
function        = "function" function_dec "{" stmt+ "}" ;
stmt            = assign | if | while | call | return | skip ;


assign          = "let" NAME "=" expr ";" ;
if              = "if" "(" cond_expr ")" "{" stmt+ "}" "else" "{" stmt+ "}" ;
while           = "while' "(" cond_expr ")" "{" stmt+ "}" ;
call            = "call" NAME ";" ;
return          = "return" expr ";" ;
skip            = "skip" ";" ;

cond_expr       = rel_expr
                | "!" cond_expr
                | "(" cond_expr ")"
                | cond_expr "&&" cond_expr
                | cond_expr "||" cond_expr ;

rel_expr        = rel_factor ">" rel_factor
                | rel_factor "<" rel_factor
                | rel_factor "<=" rel_factor
                | rel_factor ">=" rel_factor
                | rel_factor "==" rel_factor
                | rel_factor "!=" rel_factor ;

rel_factor      = expr ;

expr            = expr "+" term
                | expr "-" term
                | term ;

term            = term "*" factor
                | term "/" factor
                | term "%" factor
                | factor ;

factor          = "(" expr ")"
                | function_dec
                | NAME
                | INTEGER ;
```

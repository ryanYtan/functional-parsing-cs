# CSharp Functional Parsing
An experiment in combinator parsing in .NET C#

## Syntax Grammar
```ebnf
LETTER          = [A-Z] | [a-z] ;
DIGIT           = [0-9] ;
NZ_DIGIT        = [1-9] ;
NAME            = (LETTER | "_") (LETTER | DIGIT | "_")* ;
INTEGER         = "0" | NZDIGIT (DIGIT)* ;
STRING          = "\"" [A-Za-z0-9]* "\"" ;

args_lst        = ""
                | NAME
                | NAME "," args_lst ;

function_def    = NAME "(" args_lst ")"

program         = function+ ;
function        = "function" NAME "(" args_lst ")" "{" stmt+ "}" ;
stmt            = assign | if | while | call | return | skip ;


assign          = "let" NAME "=" (expr | STRING) ";" ;
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
                | function_def
                | NAME
                | INTEGER ;
```

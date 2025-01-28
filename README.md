# Meow
Meow is a simple language (currently it isn't one) im making in C# as a side project written completely from scratch, currently it has a regex tokenizer, 

number (10 + 3 * 2 => 16), 
boolean parsing (true or false => true) and 
string parsing ("hello" + 10 => "hello10")
comparisons (1 == 2 => false)
unary operators (-10)

I'm generating ASTs, these are the building blocks of the language, i am currently walking through the ASTs. I plan on making it similar to lua/javascript with type hinting, it's set up as a REPL (read, eval, print, loop)

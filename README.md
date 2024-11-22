# Meow
Meow is a prototype-based object oriented (OOP) scripting language that designed to be easy to integrate and embed, It is an interpreted language with static-typing, built with C#.
It takes alot of inspiration from typescript, and lua(u).
This is very much still work-in-progress, this is just a hobby project for me currently.
You mostly write the lang in camelCase, functions and objects are in PascalCase.

Here's an example of how the syntax will look (bound to change)

```
var x = 42 // Initalizes a variable with type inference
var y : int = 42 // Initalizes a variable with type annotations

// Defines a function called Main with type inference
function log(message)
{
  // Prints to the console
  print(message)
}

// Defines a function called Main with type annotations
function add(a:int, b:int) -> int
{
  return a + b;
}
```

A object is similar to a table in lua, here's an example.

```
// defines a object named Vector3
object Vector3 {
  x : float = 0
  y : float = 0
  z : float = 0
}

// Create a new instance of Vector3 with custom values
var position = new Vector3 { x = 10, y = 15, z = 20 }
```

right now there is a lexer for the project, i had an AST but i got rid of it.

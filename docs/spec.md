# Muoa Language Specification

## Introduction

Muoa, pronounced "mu-ya", is a very minimal stack-based array programming language based primarily on [Uiua](https://www.uiua.org).

Muoa is unique from other array languages in that it exclusively uses ASCII characters.

This is version 3 of the language specification.

## Terms

The "host language" is either the language the interpreter is written in, or whatever the compilation output is.

## Data types

Muoa has six different data types, *number*, *atom*, *string*, *array*, *function*, and *module*.

All data types are passed by value unless otherwise stated and all of them are first-class.

*Number* is a 64-bit floating point number.
They are formatted using the host language's built-in number to string function.

*Atom* is a pass-by-reference singleton, and separate instances of the same atom are equal (e.g. `'abs == 'abs == true`).
Atoms are declared with `'`, e.g. `'atom`.
They are formatted as `'[content]`.

*String* is a UTF-16 encoded[^1] sequence of characters which are able to be manipulated in the same ways as arrays are.
When any character in a string becomes unrepresentable by an unsigned 8-bit number, the string is automatically coerced into an array.
They are formatted as `"[content]"`.

*Array* is an arraylist (or other appliciable data structure when needed) containing other data types, including itself.
How they are formatted is up to the implementation to decide, but the Python or Golang styles should be preferred.

*Function* is a pass-by-reference singleton containing either a list of nodes/instructions or a pointer to a function written in the host language.
They are formatted as `<composite function with signature [input].[output]>` or `<builtin function with signature [input].[output]>`.

*Module* is a hashmap[^2] containing key-value pairs where the key is a string and the value is any Muoa data type.
They can be indexed into with atoms via `@`.
How they are formatted is up to the implementation to decide, but the `<module containing N entries>` is preferred for simplicity.

## Data type declaration

Numbers are defined like they normally are, e.g. `2020`, `3.14159`, `1.3333333333333`.

Atoms are defined with a leading single quote (`'`), e.g. `'deadbeef`, `'grahaTia`, `'yourMom`.

Strings are also defined like they normally are, e.g. `"I am all of me"`, `"it's hard to see your face"`, `"don't you dare look back"`.

Arrays are declared with square brackets, e.g. `[1 2 3 4]`, `[42]`, `[3.14, 1.43, 4.31]`.

Functions are defined with a signature in the format `[inputs].[outputs]` inside of a pair of parentheses, e.g. `(2.1 ,||)`, `(1.1 (2.1 +) f)`, `(3.1 + +)`.

Modules are simply any imported file, every binding inside of them is converted into a member of the module.

## Builtins

Muoa has a set of built-in operations like any other array language.

This documentation uses an altered version of Forth notation for its function sigatures.
for example, a function that takes an array containing any value and a number and returns an atom would be `{ [...] num -> atom }`, with the number being at the top of the stack.

Some operations may have alternate versions that do different things and/or take and output different values.

- `+` - Add - `{ a b -> a + b }` - Takes in two values, adds them together, then outputs the result.
- `-` - Subtract - `{ a b -> a - b }` - Takes in two values, subtracts one from the other, then outputs the result.
- `*` - Multiply - `{ a b -> a * b }` - Takes in two values, multiplies them together, then outputs the result.
- `/` - Divide - `{ a b -> a / b }` - Takes in two values, divides one with the other, then outputs the result.
- `%` - Modulus - `{ a b -> a % b }` - Takes in two values, divides one with the other, then outputs the remainder.
- `^` - Power - `{ a b -> a ^ b }` - Takes in two values, raises one to the power of the other, then outputs the result.
- `` ` `` - Negate - `{ a -> -a }` - Takes in one value, negates it, then outputs the result.
- `|` - Join - `{ [...] [...] -> [...] + [...] }` - Takes in two arrays, joins them together, then outputs the result.
- `|` - Join - `{ a b -> [a b] }` - Takes in two items, puts them into a new array, then outputs it.
- `|` - Join - `{ a [...] -> [...].Add(a) }` - Takes in an array and item, appends the items to the array, and outputs the result.
- `=` - Equal - `{ a b -> a == b }` - Takes in two values, checks their equality, then outputs the result.
- `~` - Not Equal - `{ a b -> a != b }` - Takes in two values, checks their inequality, then outputs the result.
- `>` - Less Than - `{ a b -> a > b }` - Takes in two values, compares them, then outputs the result.
- `<` - Greater Than - `{ a b -> a == b }` - Takes in two values, compares them, then outputs the result.
- `:` - Swap - `{ a b -> b a }` - Takes in two values and outputs them in reverse order.
- `.` - Dup - `{ a -> a a }` - Takes in a value and outputs two of it.
- `,` - Over - `{ a b -> a b a }` - Copies the second from top item to the top of the stack.
- `_` - Drop - `{ a -> }` - Takes in a value and outputs nothing.
- `f` - Fold - `{ [...] fun -> b }` - Takes in an array and a folding function, applies the function to the array, and outputs the result.
- `#` - Length - `{ [...] -> [...].Length }` - Takes in an array and outputs its length.
- `@` - Index - `{ a b -> a[b] }` - Takes in values A and B, then indexes into B using A.
- `$` - Slice - `{ [...] a b -> [...][a:b] }` - Takes in an array and numbers A and B, slices into the array with A and B in the form of `array[A:B]`, then outputs the result.
- `&` - Bind - `{ a atom -> }` - Takes in value A and atom B, then creates a binding[^3] of B containing A.
- `!` - Execute - `{ fun -> fun() }` - Takes in a function and calls it.
- `!` - Execute - `{ atom -> a }` - Takes in an atom, and gets the binding assigned to it in the current scope; if it can't be found in the current scope, it searches outer scopes recursively.
- `?` - Pull/Hook - `{ -> a }` - Takes a value from the surrounding scope's stack, and pushes it onto the current scope's stack.
- `s` - Switch - `{ num [...fun] -> }` - Takes in a number N and array Arr, indexes into Arr with N, then calls the result.

## Standard library

Muoa also has a set of built-in non-operation functions which are automatically loaded into the global scope at the start of runtime.

They are usually called with `[atom] !!`, e.g. `'rand !!`

They use the same notation as the operations.

- `print` - `{ a -> }` - Takes in one value then prints it with a newline.
- `import` - `{ str -> mod }` - Reads and interprets a file, captures its global binds into a module, then returns it.
- `assert` - `{ num -> }` - Takes in a number then causes an error if the number is zero.

## Footnotes

[^1]: This is simply because of how C# strings work, other implementations may use other encodings as long as they can contain UTF-8 characters.

[^2]: Using an alternative when a hashmap isn't appliciable is also valid, e.g. when the implementation is a compiler.

[^3]: A binding is an immutable scoped assignment which can't be reassigned but **can** be shadowed in a lower scope.

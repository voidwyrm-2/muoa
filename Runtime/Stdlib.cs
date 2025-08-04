using Muoa.Parsing;
using Muoa.Tokenizing;

namespace Muoa.Runtime;

public static class Stdlib
{
    public static void Import(CallingContext ctx)
    {
        var items = ctx.scope.GetExpect([MuoaType.String]);
        
        string path = ((items[0] as MuoaString)!.Value() as string)!;

        string content;
        try
        {
            content = File.ReadAllText(path);
        }
        catch (IOException e)
        {
            throw new RuntimeException(e.Message);
        }

        Lexer lexer = new(content, Path.GetFileName(path));

        var tokens = lexer.Lex();
        
        Parser parser = new(tokens);

        var nodes = parser.Parse();

        Interpreter interpreter = new(new Scope(null, false), ctx.builtins);

        interpreter.Eval(nodes);

        MuoaModule module = interpreter.Scope.ToModule();
        
        ctx.scope.Push(module);
    }

    public static void AddToScope(Scope scope)
    {
        scope.Bind("import", new BuiltinFunction(1, 1, Stdlib.Import));
    }
}
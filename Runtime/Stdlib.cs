using Muoa.Parsing;
using Muoa.Tokenizing;

namespace Muoa.Runtime;

public static class Stdlib
{
    private static void Print(CallingContext ctx)
    {
        var items = ctx.scope.GetExpect([null]);
        
        Console.WriteLine(items[0] is MuoaString str ? str.AsString() : items[0].Value());
    }
    
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

    private static Action<CallingContext> CreateAssert()
    {
        int count = 0;

        return MuoaAssert;
        
        void MuoaAssert(CallingContext ctx)
        {
            count++;
            
            var items = ctx.scope.GetExpect([MuoaType.Number]);

            if ((double)items[0].Value() == 0)
                throw new RuntimeException($"test {count} failed");
            
            Console.WriteLine($"test {count} passed");
        }
    }

    public static void AddToScope(Scope scope)
    {
        scope.Bind("print", new BuiltinFunction(1, 0, Stdlib.Print));
        scope.Bind("import", new BuiltinFunction(1, 1, Stdlib.Import));
        scope.Bind("assert", new BuiltinFunction(1, 0, Stdlib.CreateAssert()));
    }
}
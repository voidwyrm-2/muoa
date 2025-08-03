using static Muoa.Tokenizing.Lexer;

namespace Muoa.Runtime;

public static class Builtins
{
    private static void Add(CallingContext ctx)
    {
        var items = ctx.scope.GetExpect([null, null]);
        
        ctx.scope.Push(items[1].Add(items[0]));
    }
    
    private static void Div(CallingContext ctx)
    {
        var items = ctx.scope.GetExpect([null, null]);
        
        ctx.scope.Push(items[1].Div(items[0]));
    }

    private static void Length(CallingContext ctx)
    {
        var items = ctx.scope.GetExpect([null]);

        ctx.scope.Push(items[0].Length());
    }
    
    private static void Fold(CallingContext ctx)
    {
        var items = ctx.scope.GetExpect([MuoaType.Function, MuoaType.Array]);

        IMuoaFunction fun = (items[0] as IMuoaFunction)!;
        
        ctx.scope.Push(items[1].Fold(ctx, fun));
    }
    
    private static void Dup(CallingContext ctx)
    {
        var items = ctx.scope.GetExpect([null]);

        ctx.scope.Push(items[0]);
        ctx.scope.Push(items[0].Copy());
    }
    
    private static void Swap(CallingContext ctx)
    {
        var items = ctx.scope.GetExpect([null, null]);

        ctx.scope.Push(items[0]);
        ctx.scope.Push(items[1]);
    }

    public static Dictionary<Token.Type, IMuoaFunction> GetBuiltins()
    {
        Dictionary<Token.Type, IMuoaFunction> map = [];

        map[Token.Type.Add] = new BuiltinFunction(2, 1, Builtins.Add);
        map[Token.Type.Div] = new BuiltinFunction(2, 1, Builtins.Div);
        
        map[Token.Type.Length] = new BuiltinFunction(1, 1, Builtins.Length);
        map[Token.Type.Fold] = new BuiltinFunction(2, 1, Builtins.Fold);
        
        map[Token.Type.Dup] = new BuiltinFunction(1, 2, Builtins.Dup);
        map[Token.Type.Swap] = new BuiltinFunction(1, 2, Builtins.Swap);
        
        
        return map;
    }
}
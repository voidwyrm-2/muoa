using System.Diagnostics;
using static Muoa.Tokenizing.Lexer;

namespace Muoa.Runtime;

public static class Builtins
{
    private static void Add(CallingContext ctx)
    {
        var items = ctx.scope.GetExpect([null, null]);
        
        ctx.scope.Push(items[1].Add(items[0]));
    }
    
    private static void Sub(CallingContext ctx)
    {
        var items = ctx.scope.GetExpect([null, null]);
        
        ctx.scope.Push(items[1].Sub(items[0]));
    }
    
    private static void Mul(CallingContext ctx)
    {
        var items = ctx.scope.GetExpect([null, null]);
        
        ctx.scope.Push(items[1].Mul(items[0]));
    }
    
    private static void Div(CallingContext ctx)
    {
        var items = ctx.scope.GetExpect([null, null]);
        
        ctx.scope.Push(items[1].Div(items[0]));
    }
    
    private static void Mod(CallingContext ctx)
    {
        var items = ctx.scope.GetExpect([null, null]);
        
        ctx.scope.Push(items[1].Mod(items[0]));
    }
    
    private static void Pow(CallingContext ctx)
    {
        var items = ctx.scope.GetExpect([null, null]);
        
        ctx.scope.Push(items[1].Pow(items[0]));
    }
    
    private static void Join(CallingContext ctx)
    {
        var items = ctx.scope.GetExpect([null, null]);
        
        ctx.scope.Push(items[1].Join(items[0]));
    }
    
    private static void Negate(CallingContext ctx)
    {
        var items = ctx.scope.GetExpect([null]);

        ctx.scope.Push(items[0].Negate());
    }

    private static void Equal(CallingContext ctx)
    {
        var items = ctx.scope.GetExpect([null, null]);

        bool cond = items[1].Equals(items[0]);
        
        ctx.scope.Push(new MuoaNumber(cond ? 1 : 0));
    }
    
    private static void NotEqual(CallingContext ctx)
    {
        Builtins.Equal(ctx);
        
        MuoaNumber num = (MuoaNumber)ctx.scope.Pop();
        
        double? cond = num.Value() as double?;
        
        ctx.scope.Push(new MuoaNumber((int)cond! == 1 ? 0 : 1));
    }
    
    private static void Fold(CallingContext ctx)
    {
        var items = ctx.scope.GetExpect([MuoaType.Function, MuoaType.Array]);

        IMuoaFunction fun = (IMuoaFunction)items[0];
        
        ctx.scope.Push(items[1].Fold(ctx, fun));
    }
    
    private static void Length(CallingContext ctx)
    {
        var items = ctx.scope.GetExpect([null]);

        ctx.scope.Push(items[0].Length());
    }
    
    private static void Index(CallingContext ctx)
    {
        var items = ctx.scope.GetExpect([null, null]);

        IMuoaValue index = items[0];
        
        ctx.scope.Push(items[1].Index(index));
    }
    
    private static void Swap(CallingContext ctx)
    {
        var items = ctx.scope.GetExpect([null, null]);

        ctx.scope.Push(items[0]);
        ctx.scope.Push(items[1]);
    }
    
    private static void Dup(CallingContext ctx)
    {
        var items = ctx.scope.GetExpect([null]);

        ctx.scope.Push(items[0]);
        ctx.scope.Push(items[0].Copy());
    }
    
    private static void Over(CallingContext ctx)
    {
        var items = ctx.scope.GetExpect([null, null]);

        ctx.scope.Push(items[1]);
        ctx.scope.Push(items[0]);
        ctx.scope.Push(items[1].Copy());
    }

    private static void Drop(CallingContext ctx) =>
        ctx.scope.GetExpect([null]);

    private static void Bind(CallingContext ctx)
    {
        var items = ctx.scope.GetExpect([MuoaType.Atom, null]);

        IMuoaValue name = items[0];
        IMuoaValue value = items[1];
        
        ctx.scope.Bind((string)name.Value(), value);
    }
    
    private static void Execute(CallingContext ctx)
    {
        var items = ctx.scope.GetExpect([null]);

        IMuoaValue value = items[0];

        switch (value)
        {
            case IMuoaFunction func:
                func.Call(ctx);
                break;
            case MuoaAtom atom:
                ctx.scope.Push(ctx.scope.Get((string)atom.Value()));
                break;
            default:
                // Manually throw an exception for the incorrect type because
                // Scope::Expect doesn't have a way to specify multiple allowed types
                throw new RuntimeException($"Expected {MuoaType.Function} or {MuoaType.Atom} in position 1 on the stack, but found {value.Type()} instead");
        }
    }

    private static void Switch(CallingContext ctx)
    {
        var items = ctx.scope.GetExpect([MuoaType.Array, MuoaType.Number]);

        double value = (double)items[1].Value();

        int index = (int)value!;

        if ($"{index}" != $"{value}")
            throw new RuntimeException("Switch only accepts whole numbers");

        var options = (IMuoaValue[])items[0].Value();

        if (options.Length < index)
            throw new RuntimeException($"Switch index {index} is out of bounds for array of size {options.Length}");
        
        IMuoaValue selected = options[index];

        if (selected is IMuoaFunction fun)
            fun.Call(ctx);
        else
            throw new RuntimeException($"Switch expects a {MuoaType.Function}, but found {selected.Type()} instead");
    }

    public static Dictionary<Token.Type, IMuoaFunction> GetBuiltins()
    {
        Dictionary<Token.Type, IMuoaFunction> map = [];

        map[Token.Type.Add] = new BuiltinFunction(2, 1, Builtins.Add);
        map[Token.Type.Sub] = new BuiltinFunction(2, 1, Builtins.Sub);
        map[Token.Type.Mul] = new BuiltinFunction(2, 1, Builtins.Mul);
        map[Token.Type.Div] = new BuiltinFunction(2, 1, Builtins.Div);
        map[Token.Type.Mod] = new BuiltinFunction(2, 1, Builtins.Mod);
        map[Token.Type.Pow] = new BuiltinFunction(2, 1, Builtins.Pow);
        map[Token.Type.Join] = new BuiltinFunction(2, 1, Builtins.Join);
        map[Token.Type.Negate] = new BuiltinFunction(2, 1, Builtins.Negate);

        map[Token.Type.Equal] = new BuiltinFunction(2, 1, Builtins.Equal);
        map[Token.Type.NotEqual] = new BuiltinFunction(2, 1, Builtins.NotEqual);
        
        map[Token.Type.Fold] = new BuiltinFunction(2, 1, Builtins.Fold);
        map[Token.Type.Length] = new BuiltinFunction(1, 1, Builtins.Length);
        map[Token.Type.Index] = new BuiltinFunction(1, 1, Builtins.Index);
        
        map[Token.Type.Swap] = new BuiltinFunction(1, 2, Builtins.Swap);
        map[Token.Type.Dup] = new BuiltinFunction(1, 2, Builtins.Dup);
        map[Token.Type.Over] = new BuiltinFunction(2, 3, Builtins.Over);
        map[Token.Type.Drop] = new BuiltinFunction(1, 2, Builtins.Drop);
        
        map[Token.Type.Bind] = new BuiltinFunction(1, 2, Builtins.Bind);
        map[Token.Type.Execute] = new BuiltinFunction(1, 0, Builtins.Execute);
        
        map[Token.Type.Switch] = new BuiltinFunction(2, 0, Builtins.Switch);
        
        return map;
    }
}
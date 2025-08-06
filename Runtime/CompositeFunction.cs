using Muoa.Parsing;

namespace Muoa.Runtime;

public class CompositeFunction(int input, int output, INode[] nodes) : IMuoaFunction
{
    public CompositeFunction(FunctionNode node) : this(node.input, node.output, node.nodes) { }
    
    public (int, int) Signature() => (input, output);

    public MuoaType Type() => MuoaType.Function;

    public IMuoaValue Copy() => this;
    
    public IMuoaValue Default() => new CompositeFunction(0, 0, []);

    public object Value() => nodes;
    
    public void Call(CallingContext ctx)
    {
        Scope innerScope = ctx.direct ? ctx.scope : new Scope(ctx.scope, true);
        
        innerScope.Bind("me", this);
        Stdlib.AddToScope(innerScope);

        var inputs = ctx.scope.GetExpect(new MuoaType?[input]);

        foreach (IMuoaValue value in inputs.Reverse())
            innerScope.Push(value);
        
        Interpreter interpreter = new(innerScope, ctx.builtins);

        interpreter.Eval(nodes);
        
        var outputs = innerScope.GetExpect(new MuoaType?[output]);
        
        foreach (IMuoaValue value in outputs.Reverse())
            ctx.scope.Push(value);
    }

    public bool Equals(IMuoaValue? other) => Utils.DefaultValueEquals(other);

    public override string ToString() => $"<composite function with signature {input}.{output}>";
}
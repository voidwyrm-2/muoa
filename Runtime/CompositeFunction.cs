using Muoa.Parsing;

namespace Muoa.Runtime;

public class CompositeFunction(int input, int output, INode[] nodes) : IMuoaFunction
{
    public MuoaType Type() => MuoaType.Function;

    public IMuoaValue Copy() => this;
    
    public IMuoaValue Default() => new CompositeFunction(0, 0, []);

    public object Value() => nodes;

    public CompositeFunction(FunctionNode node) : this(node.input, node.output, node.nodes) { }
    
    public void Call(CallingContext ctx)
    {
        Scope innerScope = ctx.direct ? ctx.scope : new Scope(ctx.scope, true);
        Interpreter interpreter = new(innerScope, ctx.builtins);

        interpreter.Eval(nodes);
        
        innerScope.Expect(new MuoaType?[output]);
        
        for (int i = 0; i < output; i++)
            ctx.scope.Push(innerScope.Pop());
    }

    public (int, int) Signature() => (input, output);

    public override string ToString() => $"<composite function with signature {input}.{output}>";
}
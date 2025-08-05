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

        var inputs = new IMuoaValue[input];

        for (int i = 0; i < input; i++)
            inputs[i] = ctx.scope.Pop();

        foreach (IMuoaValue value in inputs.Reverse())
            innerScope.Push(value);
        
        Interpreter interpreter = new(innerScope, ctx.builtins);

        interpreter.Eval(nodes);
        
        innerScope.Expect(new MuoaType?[output]);
        
        for (int i = 0; i < output; i++)
            ctx.scope.Push(innerScope.Pop());
    }

    public bool Equals(IMuoaValue? other) => Utils.DefaultValueEquals(other);

    public override string ToString() => $"<composite function with signature {input}.{output}>";
}
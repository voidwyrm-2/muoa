namespace Muoa.Runtime;

public class BuiltinFunction(int input, int output, Action<CallingContext> fun) : IMuoaFunction
{
    public MuoaType Type() => MuoaType.Function;

    public IMuoaValue Copy() => this;
    
    public IMuoaValue Default() => throw new NotImplementedException();

    public object Value() => fun;

    public void Call(CallingContext ctx) => fun(ctx);
    
    public (int, int) Signature() => (input, output);
    
    public override string ToString() => $"<builtin function with signature {input}.{output}>";
}
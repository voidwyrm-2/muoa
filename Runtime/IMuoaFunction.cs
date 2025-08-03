namespace Muoa.Runtime;

public interface IMuoaFunction : IMuoaValue
{
    public void Call(CallingContext ctx);
    
    public (int, int) Signature();
}
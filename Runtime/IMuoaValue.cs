namespace Muoa.Runtime;

public interface IMuoaValue 
{
    public MuoaType Type();
    
    public IMuoaValue Copy();

    public IMuoaValue Default();
    
    public object Value();

    public IMuoaValue Length() =>
        this.InvalidUnaryOp("length");
    
    public IMuoaValue Index(IMuoaValue index) =>
        this.InvalidUnaryOp("index");

    public IMuoaValue Reduce(CallingContext ctx, IMuoaFunction fun) =>
        this.InvalidUnaryOp("reduce");
    
    public IMuoaValue Add(IMuoaValue other) =>
        this.InvalidBinaryOp(other, "add");
    
    public IMuoaValue Sub(IMuoaValue other) =>
        this.InvalidBinaryOp(other, "sub");
    
    public IMuoaValue Mul(IMuoaValue other) =>
        this.InvalidBinaryOp(other, "mul");
    
    public IMuoaValue Div(IMuoaValue other) =>
        this.InvalidBinaryOp(other, "div");
    
    public IMuoaValue Mod(IMuoaValue other) =>
        this.InvalidBinaryOp(other, "mod");
    
    public IMuoaValue Pow(IMuoaValue other) =>
        this.InvalidBinaryOp(other, "pow");
    
    public IMuoaValue Join(IMuoaValue other) =>
        this.InvalidBinaryOp(other, "join");
}

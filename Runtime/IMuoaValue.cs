using System.Diagnostics;

namespace Muoa.Runtime;

public interface IMuoaValue : IEquatable<IMuoaValue>
{
    public MuoaType Type();
    
    public IMuoaValue Copy();

    public IMuoaValue Default();
    
    public object Value();

    public IMuoaValue Length() =>
        this.InvalidUnaryOp("length");
    
    public IMuoaValue Index(IMuoaValue index) =>
        this.InvalidIndexingOp(index);

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

    // isn't a parent knowing of its children an antipattern/code smell?
    public IMuoaValue Join(IMuoaValue other) => other switch
    {
        MuoaArray arr => new MuoaArray(new[]{this}.Concat(arr).ToArray()),
        _ => new MuoaArray([this, other])
    };
    
    public IMuoaValue Negate() =>
        this.InvalidUnaryOp("negate");
}

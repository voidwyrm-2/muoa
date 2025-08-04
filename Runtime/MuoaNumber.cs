namespace Muoa.Runtime;

public class MuoaNumber(float value) : IMuoaValue
{
    private readonly float _value = value;
    
    public MuoaType Type() => MuoaType.Number;

    public IMuoaValue Copy() => new MuoaNumber(_value);
    
    public IMuoaValue Default() => new MuoaNumber(0);
    
    public object Value() => _value;

    public IMuoaValue Add(IMuoaValue other) => other switch
    {
        MuoaNumber num => new MuoaNumber(_value + num._value),
        MuoaArray arr => new MuoaArray(arr.Select(item => item.Add(this)).ToArray()),
        _ => this.InvalidBinaryOp(other, "add")
    };
    
    public IMuoaValue Sub(IMuoaValue other) => other switch
    {
        MuoaNumber num => new MuoaNumber(_value - num._value),
        MuoaArray arr => new MuoaArray(arr.Select(item => item.Sub(this)).ToArray()),
        _ => this.InvalidBinaryOp(other, "sub")
    };
    
    public IMuoaValue Mul(IMuoaValue other) => other switch
    {
        MuoaNumber num => new MuoaNumber(_value * num._value),
        MuoaArray arr => new MuoaArray(arr.Select(item => item.Mul(this)).ToArray()),
        _ => this.InvalidBinaryOp(other, "mul")
    };
    
    public IMuoaValue Div(IMuoaValue other) => other switch
    {
        MuoaNumber num => new MuoaNumber(_value / num._value),
        MuoaArray arr => new MuoaArray(arr.Select(item => item.Div(this)).ToArray()),
        _ => this.InvalidBinaryOp(other, "div")
    };

    public override string ToString() => $"{_value}";
}
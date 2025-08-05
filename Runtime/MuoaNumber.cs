namespace Muoa.Runtime;

public class MuoaNumber(double value) : IMuoaValue
{
    private readonly double _value = value;
    
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

    public bool Equals(IMuoaValue? other) => other switch
    {
        MuoaNumber num => Math.Abs(_value - num._value) < 0.001,
        _ => Utils.DefaultValueEquals(other)
    };

    public override string ToString() => $"{_value}";
}
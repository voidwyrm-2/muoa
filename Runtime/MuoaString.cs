namespace Muoa.Runtime;

public class MuoaString(string value) : IMuoaValue
{
    private readonly string _value = value;
    
    public MuoaType Type() => MuoaType.String;

    public IMuoaValue Copy() => new MuoaString(_value);
    
    public IMuoaValue Default() => new MuoaString("");

    public object Value() => _value;

    public override string ToString() => $"\"{_value}\"";
}
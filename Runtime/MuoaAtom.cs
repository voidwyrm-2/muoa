namespace Muoa.Runtime;

public class MuoaAtom(string value) : IMuoaValue, IEquatable<IMuoaValue>
{
    private readonly string _value = value;

    public MuoaType Type() => MuoaType.Atom;

    public IMuoaValue Copy() => this;

    public IMuoaValue Default() => throw new NotImplementedException();

    public object Value() => _value;

    public bool Equals(IMuoaValue? other) => other is MuoaAtom atom && atom._value == _value;
    
    public override string ToString() => $"'{_value}";
}
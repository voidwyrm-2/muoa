using System.Collections;
using System.Text;

namespace Muoa.Runtime;

public class MuoaString(string value) : IMuoaValue, IEnumerable<byte>, IEnumerable<char>, IEnumerable<IMuoaValue>
{
    private readonly string _value = value;
    
    public MuoaType Type() => MuoaType.String;

    public IMuoaValue Copy() => new MuoaString(_value);
    
    public IMuoaValue Default() => new MuoaString("");

    public object Value() => _value;
    
    public bool Equals(IMuoaValue? other) => other switch
    {
        MuoaString str => _value == str._value,
        _ => Utils.DefaultValueEquals(other)
    };
    
    IEnumerator<char> IEnumerable<char>.GetEnumerator() =>
        _value.AsEnumerable().GetEnumerator();

    public IEnumerator<byte> GetEnumerator() =>
        (IEnumerator<byte>)Encoding.ASCII.GetBytes(_value).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IEnumerator<IMuoaValue> IEnumerable<IMuoaValue>.GetEnumerator() =>
        (IEnumerator<IMuoaValue>)this.Select<char, IMuoaValue>(ch => new MuoaNumber(ch));
    
    public override string ToString() => $"\"{_value}\"";
}
using System.Collections;
using System.Text;

namespace Muoa.Runtime;

public class MuoaString(byte[] value) : IMuoaValue, IEnumerable<byte>, IEnumerable<IMuoaValue>
{
    public MuoaString(string value) : this(Encoding.Default.GetBytes(value)) { }
    
    private readonly byte[] _value = value;
    
    public MuoaType Type() => MuoaType.String;

    public IMuoaValue Copy() => new MuoaString((byte[])_value.Clone());
    
    public IMuoaValue Default() => new MuoaString(Array.Empty<byte>());

    public object Value() => _value;

    public IMuoaValue Length() => new MuoaNumber(_value.Length);

    public bool Equals(IMuoaValue? other) => other switch
    {
        MuoaString str => AsString() == str.AsString(),
        _ => Utils.DefaultValueEquals(other)
    };

    public IEnumerator<byte> GetEnumerator() => (IEnumerator<byte>)_value.GetEnumerator();

    IEnumerator<IMuoaValue> IEnumerable<IMuoaValue>.GetEnumerator() =>
        this.Select<byte, IMuoaValue>(ch => new MuoaNumber(ch))
            .GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    public string AsString() => Encoding.Default.GetString(_value);
    
    public override string ToString() => $"\"{AsString()}\"";
}
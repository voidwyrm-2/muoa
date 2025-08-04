using System.Collections;
using System.Text;

namespace Muoa.Runtime;

public class MuoaString(string value) : IMuoaValue, IEnumerable<byte>
{
    private readonly string _value = value;
    
    public MuoaType Type() => MuoaType.String;

    public IMuoaValue Copy() => new MuoaString(_value);
    
    public IMuoaValue Default() => new MuoaString("");

    public object Value() => _value;

    public IEnumerator<byte> GetEnumerator() => (IEnumerator<byte>)Encoding.ASCII.GetBytes(_value).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString() => $"\"{_value}\"";
}
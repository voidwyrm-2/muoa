using System.Collections;
using System.Diagnostics;

namespace Muoa.Runtime;

public class MuoaArray(IMuoaValue[] value) : IMuoaValue, IEnumerable<IMuoaValue>
{
    private readonly IMuoaValue[] _value = value;
    
    public MuoaType Type() => MuoaType.Array;

    public IMuoaValue Copy() =>
        new MuoaArray(_value.Select(item => item.Copy()).ToArray());

    public IMuoaValue Default() => new MuoaArray([]);

    public object Value() => _value;

    public IMuoaValue Length() => new MuoaNumber(_value.Length);

    public IMuoaValue Join(IMuoaValue other) => other switch
    {
        IEnumerable<IMuoaValue> arr => new MuoaArray(_value.Concat(arr).ToArray()),
        _ => new MuoaArray(_value.Append(other).ToArray()),
    };

    public bool Equals(IMuoaValue? other) => other switch
    {
        null => throw new UnreachableException("null should not be passed to IMuoaValue::Equals"),
        IEnumerable<IMuoaValue> arr => this.Zip(arr).All(tup => tup.First.Equals(tup.Second)),
        _ => _value.All(item => item.Equals(other))
    };

    public IEnumerator<IMuoaValue> GetEnumerator() => ((IEnumerable<IMuoaValue>)_value).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    public override string ToString() => "["+ string.Join(", ", _value.Select(item => item.ToString())) + "]";
}
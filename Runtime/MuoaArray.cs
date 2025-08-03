using System.Collections;

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

    public IMuoaValue Reduce(CallingContext ctx, IMuoaFunction fun)
    {
        fun.ExpectSignature(2, 1);
        
        Scope accScope = new(ctx.scope, true);

        bool evenLength = _value.Length % 2 == 0;

        accScope.Push(evenLength ? _value[0] : _value[0].Default());

        for (int i = evenLength ? 1 : 0; i < _value.Length; i += 1)
        {
            accScope.Push(_value[i]);
            fun.Call(new CallingContext(ctx.builtins, accScope, true));
        }

        return accScope.Pop();
    }

    public IEnumerator<IMuoaValue> GetEnumerator() => ((IEnumerable<IMuoaValue>)_value).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    public override string ToString() => string.Join(", ", _value.Select(item => item.ToString()));
}
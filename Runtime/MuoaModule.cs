namespace Muoa.Runtime;

public class MuoaModule(Dictionary<string, IMuoaValue> inner) : IMuoaValue
{
    public MuoaType Type() => MuoaType.Module;

    public IMuoaValue Copy() => this;

    public IMuoaValue Default() => throw new NotImplementedException();

    public object Value() => inner;

    public IMuoaValue Index(IMuoaValue index) => index switch
    {
        _ => this.InvalidIndexingOp(index)
    };

    public IMuoaValue Length() => new MuoaNumber(inner.Count);

    public bool Equals(IMuoaValue? other) => Utils.DefaultValueEquals(other);

    public override string ToString() => $"<module with {inner.Count} entries>";
}
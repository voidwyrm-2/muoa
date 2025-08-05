namespace Muoa.Runtime;

public class Scope(Scope? parent, bool function)
{
    public readonly Scope? parent = parent;
    public readonly Stack<IMuoaValue> stack = [];
    private readonly Dictionary<string, IMuoaValue> _bindings = [];
    
    public void Push(IMuoaValue value) => stack.Push(value);
    
    public IMuoaValue Pop()
    {
        if (stack.Count == 0)
            throw new RuntimeException("Stack underflow");
        
        return stack.Pop();
    }

    public void Expect(MuoaType?[] expected)
    {
        if (stack.Count < expected.Length)
            throw new RuntimeException($"Expected {expected.Length} items on the stack, but found {stack.Count} instead");
        
        var stackArr = stack.ToArray();
        
        for (int i = 0; i < expected.Length; i++)
        {
            if (stackArr[i].Type() != expected[i] && expected[i] != null)
                throw new RuntimeException($"Expected {(expected[i] == null ? "value" : expected[i])} in position {i + 1} on the stack, but found {stackArr[i].Type()} instead");
        }
    }

    public IMuoaValue[] GetExpect(MuoaType?[] expected)
    {
        Expect(expected);
        
        var newArr = new IMuoaValue[expected.Length];

        for (int i = 0; i < expected.Length; i++)
            newArr[i] = stack.Pop();

        return newArr;
    }

    public IMuoaValue Get(string name)
    {
        if (_bindings.TryGetValue(name, out IMuoaValue? value))
            return value;

        if (parent != null)
            return parent.Get(name);
        
        throw new RuntimeException($"'{name}' does not exist");
    }

    public void Bind(string name, IMuoaValue value)
    {
        if (!_bindings.TryAdd(name, value))
            throw new RuntimeException($"'{name}' already exists");
    }

    public MuoaModule ToModule() => new(_bindings);
}
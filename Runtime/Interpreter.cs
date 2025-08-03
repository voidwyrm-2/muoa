using Muoa.Parsing;
using Muoa.Tokenizing;

namespace Muoa.Runtime;

public class Interpreter(Scope? scope, Dictionary<Lexer.Token.Type, IMuoaFunction> builtins)
{
    private Scope _scope = scope ?? new Scope(null, false);
    private readonly Dictionary<Lexer.Token.Type, IMuoaFunction> _builtins = builtins;

    public Interpreter(Scope? scope = null) : this(scope, Builtins.GetBuiltins()) { }

    public IMuoaValue[] Eval(INode[] nodes)
    {
        foreach (INode node in nodes)
        {
            try
            {
                switch (node)
                {
                    case NumberNode num:
                        _scope.Push(new MuoaNumber(float.Parse(num.value.lit)));
                        break;
                    case AtomNode atom:
                        _scope.Push(new MuoaAtom(atom.value.lit));
                        break;
                    case StringNode str:
                        _scope.Push(new MuoaString(str.value.lit));
                        break;
                    case ArrayNode array:
                    {
                        _scope = new Scope(_scope, false);

                        var result = Eval(array.nodes);

                        _scope = _scope.parent!;

                        var arrayResult = result.Reverse().ToArray();
                        _scope.Push(new MuoaArray(arrayResult!));
                    }
                        break;
                    case FunctionNode func:
                        _scope.Push(new CompositeFunction(func));
                        break;
                    case OperationNode op:
                        _builtins[op.op.type].Call(new CallingContext(_builtins, _scope));
                        break;
                }
            }
            catch (MuoaException e)
            {
                node.GetToken().Error(e.Message);
            }
        }
        
        return _scope.stack.ToArray();
    }
}
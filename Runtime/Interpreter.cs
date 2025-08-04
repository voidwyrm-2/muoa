using Muoa.Parsing;
using Muoa.Tokenizing;

namespace Muoa.Runtime;

public class Interpreter(Scope? scope, Dictionary<Lexer.Token.Type, IMuoaFunction> builtins)
{
    public Scope Scope { get; private set; } = scope ?? new Scope(null, false);

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
                        Scope.Push(new MuoaNumber(float.Parse(num.value.lit)));
                        break;
                    case AtomNode atom:
                        Scope.Push(new MuoaAtom(atom.value.lit));
                        break;
                    case StringNode str:
                        Scope.Push(new MuoaString(str.value.lit));
                        break;
                    case ArrayNode array:
                    {
                        Scope = new Scope(Scope, false);

                        var result = Eval(array.nodes);

                        Scope = Scope.parent!;

                        var arrayResult = result.Reverse().ToArray();
                        Scope.Push(new MuoaArray(arrayResult!));
                    }
                        break;
                    case FunctionNode func:
                        Scope.Push(new CompositeFunction(func));
                        break;
                    case OperationNode op:
                        builtins[op.op.type].Call(new CallingContext(builtins, Scope));
                        break;
                }
            }
            catch (MuoaException e)
            {
                node.GetToken().Error<RuntimeException>(e.Message);
            }
        }
        
        return Scope.stack.ToArray();
    }
}
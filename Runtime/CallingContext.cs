using Muoa.Tokenizing;

namespace Muoa.Runtime;

public struct CallingContext(Dictionary<Lexer.Token.Type, IMuoaFunction> builtins, Scope scope, bool direct = false)
{
    public readonly Dictionary<Lexer.Token.Type, IMuoaFunction> builtins = builtins;
    public readonly Scope scope = scope;
    public readonly bool direct = direct;
}
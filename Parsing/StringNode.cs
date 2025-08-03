using Muoa.Tokenizing;

namespace Muoa.Parsing;

public readonly struct StringNode(Lexer.Token value) : INode
{
    public readonly Lexer.Token value = value;
    
    public Lexer.Token GetToken() => value;
    
    public override string ToString() => $"StringNode({value})";
}
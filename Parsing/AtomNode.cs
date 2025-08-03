using Muoa.Tokenizing;

namespace Muoa.Parsing;

public readonly struct AtomNode(Lexer.Token value) : INode
{
    public readonly Lexer.Token value = value;

    public Lexer.Token GetToken() => value;
    
    public override string ToString() => $"AtomNode({value})";
}
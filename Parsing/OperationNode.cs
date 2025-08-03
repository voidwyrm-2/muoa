using Muoa.Tokenizing;

namespace Muoa.Parsing;

public readonly struct OperationNode(Lexer.Token op) : INode
{
    public readonly Lexer.Token op = op;
    
    public Lexer.Token GetToken() => op;
    
    public override string ToString() => $"OperationNode({op})";
}
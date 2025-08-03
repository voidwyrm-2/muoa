using Muoa.Tokenizing;

namespace Muoa.Parsing;

public readonly struct ArrayNode(INode[] nodes) : INode
{
    public readonly INode[] nodes = nodes;
    
    public Lexer.Token GetToken() => nodes[0].GetToken();
    
    public override string ToString() => $"ArrayNode([{nodes.Length}])";
}
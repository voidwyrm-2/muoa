using Muoa.Tokenizing;

namespace Muoa.Parsing;

public struct FunctionNode(int input, int output, INode[] nodes) : INode
{
    public readonly int input = input;
    public readonly int output = output;
    public readonly INode[] nodes = nodes;
    
    public Lexer.Token GetToken() => nodes[0].GetToken();
    
    public override string ToString() => $"FunctionNode([{nodes.Length}] with signature {input}.{output})";
}
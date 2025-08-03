using Muoa.Tokenizing;

namespace Muoa.Parsing;

public interface INode
{
    public Lexer.Token GetToken();
}
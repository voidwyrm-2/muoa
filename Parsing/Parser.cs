using System.Diagnostics;
using Muoa.Tokenizing;

namespace Muoa.Parsing;

using static Lexer;

public class Parser(Token[] tokens)
{
    private int _idx;

    private static void ThrowUnexpectedToken(Token tok) =>
        tok.Error($"Unexpected token {tok.Literal()}");

    private INode ParseFunction(Token parent)
    {
        _idx++;

        if (_idx >= tokens.Length)
            tokens.Last().Error($"Unexpected function signature, but found EOL instead");
        
        Token tok = tokens[_idx];
        
        if (tok.type != Token.Type.Number)
            tok.Error($"Unexpected function signature, but found '{tok.Literal()}' instead");

        _idx++;
        
        string[] parts = tok.Literal().Split('.');

        int input = int.Parse(parts[0]);
        int output = 0;
        
        if (parts.Length > 1)
            output = int.Parse(parts[1]);

        return new FunctionNode(input, output, Parse(parent));
    }
    
    private INode ParseArray(Token parent)
    {
        _idx++;
        return new ArrayNode(Parse(parent));
    }

    private INode[] Parse(Token? parent)
    {
        List<INode> nodes = [];

        while (_idx < tokens.Length)
        {
            Token tok = tokens[_idx];

            switch (tok.type)
            {
                case Token.Type.ParenRight when parent is { type: Token.Type.ParenLeft }:
                    return nodes.ToArray();
                case Token.Type.ParenRight:
                    Parser.ThrowUnexpectedToken(tok);
                    break;
                case Token.Type.BracketRight when parent is { type: Token.Type.BracketLeft }:
                    return nodes.ToArray();
                case Token.Type.BracketRight:
                    Parser.ThrowUnexpectedToken(tok);
                    break;
            }

            INode node = tok.type switch
            {
                Token.Type.Number => new NumberNode(tok),
                Token.Type.String => new NumberNode(tok),
                Token.Type.Atom => new AtomNode(tok),
                Token.Type.ParenLeft => ParseFunction(tok),
                Token.Type.BracketLeft => ParseArray(tok),
                _ => new OperationNode(tok)
            };

            _idx++;
            
            nodes.Add(node);
        }

        return nodes.ToArray();
    }

    public INode[] Parse() => Parse(null);
}
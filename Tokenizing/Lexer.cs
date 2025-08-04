using System.Diagnostics;
using System.Text;

namespace Muoa.Tokenizing;

public class Lexer
{
    public readonly struct Token(Token.Type type, Token.Ctx ctx, string lit, int idx, int len, int ln, int col)
    {
        public readonly Type type = type;
        public readonly string lit = lit;

        public readonly struct Ctx(string file, string line, int lineStart)
        {
            public readonly string file = file;
            public readonly string line = line;
            public readonly int lineStart = lineStart;
        }
        
        public enum Type
        {
            Number,
            String,
            Atom,
            Fold,
            Length,
            Index,
            Swap,
            Dup,
            Drop,
            Equal,
            NotEqual,
            LessThan,
            GreaterThan,
            Add,
            Sub,
            Mul,
            Div,
            Mod,
            Pow,
            Negate,
            ParenLeft,
            ParenRight,
            BracketLeft,
            BracketRight,
            Execute,
            Bind,
            Slice,
            Join,
            Pull
        }

        public string Literal()
        {
            return type switch
            {
                Type.Number => lit,
                Type.String => $"\"{lit}\"",
                Type.Atom => $"'{lit}",
                Type.Fold => ";",
                Type.Length => "#",
                Type.Index => "@",
                Type.Swap => ":",
                Type.Dup => ".",
                Type.Drop => "_",
                Type.Equal => "=",
                Type.NotEqual => "~",
                Type.LessThan => "<",
                Type.GreaterThan => ">",
                Type.Add => "+",
                Type.Sub => "-",
                Type.Mul => "*",
                Type.Div => "/",
                Type.Mod => "%",
                Type.Pow => "^",
                Type.Negate => "`",
                Type.ParenLeft => "(",
                Type.ParenRight => ")",
                Type.BracketLeft => "[",
                Type.BracketRight => "]",
                Type.Execute => "!",
                Type.Bind => "&",
                Type.Slice => "$",
                Type.Join => "|",
                Type.Pull => "?",
                _ => throw new UnreachableException($"with {type} of Type"),
            };
        }
        
        public void Error<T>(string msg) where T : MuoaException, new()
        {
            StringBuilder builder = new($"{msg}\n{ctx.file}:{ln}:{col}\n{ctx.line}\n");

            for (int i = 0; i < idx - ctx.lineStart; i++)
                builder.Append(' ');
            
            builder.Append('^');
            
            for (int i = 1; i < len && i < ctx.line.Length; i++)
                builder.Append('~');
            
            throw (T)Activator.CreateInstance(typeof(T), builder.ToString())!;
        }

        public override string ToString() => $"<{type}, '{lit}', {idx} ({len}), {col}, {ln}, '{ctx.file}'>";
    }
    
    private string _text;
    private string _file;
    private int _lineStart;
    private string _line;
    private int _idx;
    private int _col = 1;
    private int _ln = 1;
    
    private bool Eof => _idx >= _text.Length;
    
    private char Ch => _text[_idx];
    
    private char Peek(int offset = 1) => _text[_idx + offset];

    public Lexer(string text, string file)
    {
        _text = text;
        _file = file;
        _line = "";
        GetLine();
    }

    private void Advance()
    {
        _idx++;
        _col++;

        if (Eof || Ch != '\n')
            return;
        
        _lineStart = _idx + 1;
        _ln++;
        _col = 1;
        GetLine();
    }

    private void GetLine()
    {
        int start = _lineStart;
        int end = _lineStart;

        for (; end < _text.Length && _text[end] != '\n'; end++);
        
        _line = _text[start..end];
    }

    private Token.Ctx GetCtx() => new(_file, _line, _lineStart);

    private void Throw(string msg, int len = 1)
    {
        Token t = new(Token.Type.Add, GetCtx(), "", _idx, len, _ln, _col);
        t.Error<LexerException>(msg); 
    }

    private static Token.Type? GetTokenType(char ch) => ch switch
    {
        ';' => Token.Type.Fold,
        '#' =>  Token.Type.Length,
        '@' =>  Token.Type.Index,
        ':' =>  Token.Type.Swap,
        '.' =>  Token.Type.Dup,
        '_' =>  Token.Type.Drop,
        '=' =>  Token.Type.Equal,
        '~' =>  Token.Type.NotEqual,
        '<' =>  Token.Type.LessThan,
        '>' =>  Token.Type.GreaterThan,
        '+' =>  Token.Type.Add,
        '-' =>  Token.Type.Sub,
        '*' =>  Token.Type.Mul,
        '/' =>  Token.Type.Div,
        '%' =>  Token.Type.Mod,
        '^' => Token.Type.Pow,
        '`' => Token.Type.Negate,
        '(' => Token.Type.ParenLeft,
        ')' => Token.Type.ParenRight,
        '[' => Token.Type.BracketLeft,
        ']' => Token.Type.BracketRight,
        '!' => Token.Type.Execute,
        '&' => Token.Type.Bind,
        '$' => Token.Type.Slice,
        '|' => Token.Type.Join,
        '?' => Token.Type.Pull, 
        _ => null,
    };

    private Token CollectAtom()
    {
        int startidx = _idx;
        int startcol = _col;
        int startln = _ln;
        int origLineStart = _lineStart;
        string origLine = _line;
       
        StringBuilder str = new();

        Advance();

        while (!Eof && Ch is >= 'a' and <= 'z' or >= 'A' and <= 'Z')
        {
            str.Append(Ch);
            Advance();
        }

        if (str.Length == 0 && Lexer.GetTokenType(Ch) != null)
        {
            str.Append(Ch);
            Advance();
        }

        if (str.Length == 0)
        {
            Token t = new(Token.Type.Atom, new Token.Ctx(_file, origLine, origLineStart), str.ToString(), startidx, str.Length, startln, startcol);
            t.Error<LexerException>("Atoms cannot contain zero characters");
        }
        
        return new Token(Token.Type.Atom, new Token.Ctx(_file, origLine, origLineStart), str.ToString(), startidx, str.Length, startln, startcol);
    }
    
    private Token CollectNumber()
    {
        int startidx = _idx;
        int startcol = _col;
        int startln = _ln;
        int origLineStart = _lineStart;
        string origLine = _line;
       
        StringBuilder str = new();

        bool dot = false;

        while (!Eof && Ch is var ch&& (char.IsAsciiDigit(ch) || ch == '.'))
        {
            if (ch == '.')
            {
                if (dot || !char.IsAsciiDigit(Peek()))
                    break;
                
                dot = true;
            }
            
            str.Append(ch);
            Advance();
        }

        return new Token(Token.Type.Number, new Token.Ctx(_file, origLine, origLineStart), str.ToString(), startidx, str.Length, startln, startcol);
    }
    
    private Token CollectString()
    {
        int startidx = _idx;
        int startcol = _col;
        int startln = _ln;
        int origLineStart = _lineStart;
        string origLine = _line;
       
        StringBuilder str = new();
        bool escaped = false;
        
        Advance();

        while (!Eof)
        {
            char ch = Ch;
            
            if (escaped)
            {
                char? esc = ch switch
                {
                    '\\' or '\'' or '"' => ch,
                    _ => null,
                };
                
                if (esc == null)
                    Throw($"invalid escape character '{ch}'");
                
                str.Append(esc!);

                escaped = false;
            }
            else if (ch == '\\')
            {
                escaped = true;
            }
            else if (ch == '"')
            {
                break;
            }
            else
            {
                str.Append(ch);
            }
            
            Advance();
        }

        if (Eof || Ch != '"')
        {
            Token t = new(Token.Type.String, new Token.Ctx(_file, origLine, origLineStart), str.ToString(), startidx, str.Length, startln, startcol);
            t.Error<LexerException>("Unterminated string literal");
        }

        Advance();
        
        return new Token(Token.Type.String, new Token.Ctx(_file, origLine, origLineStart), str.ToString(), startidx, str.Length, startln, startcol); 
    }
    
    public Token[] Lex()
    {
        List<Token> tokens = [];

        while (!Eof)
        {
            char ch = Ch;
            
            if (char.IsWhiteSpace(ch))
            {
                Advance();
            }
            else if (ch == '\\')
            {
                while (!Eof && Ch != '\n')
                    Advance();
            }
            else if (ch == '"')
            {
                tokens.Add(CollectString());
            }
            else if (ch == '\'')
            {
                tokens.Add(CollectAtom());
            }
            else if (char.IsAsciiDigit(ch))
            {
                tokens.Add(CollectNumber());
            }
            else if (Lexer.GetTokenType(ch) is { } tt)
            {
                tokens.Add(new Token(tt, GetCtx(), ch.ToString(), _idx, 1, _ln, _col));
                Advance();
            }
            else
            {
                Throw($"Invalid character '{ch}'");
            }
        }
        
        return tokens.ToArray();
    }
}

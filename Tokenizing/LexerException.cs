namespace Muoa.Tokenizing;

public class LexerException(string msg) : MuoaException(msg)
{
    public LexerException() : this("") { }
}
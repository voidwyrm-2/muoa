namespace Muoa.Runtime;

public class RuntimeException(string msg) : MuoaException(msg)
{
    public RuntimeException() : this("") { }
}
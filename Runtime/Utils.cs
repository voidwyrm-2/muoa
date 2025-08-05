using System.Diagnostics;

namespace Muoa.Runtime;

internal static class Utils
{
    internal static IMuoaValue InvalidUnaryOp(this IMuoaValue value, string name) =>
        throw new RuntimeException($"Cannot use operation '{name}' on type {value.Type()}");
    
    internal static IMuoaValue InvalidBinaryOp(this IMuoaValue value, IMuoaValue other, string name) =>
        throw new RuntimeException($"Cannot use operation '{name}' on types {value.Type()} and {other.Type()}");
    
    internal static IMuoaValue InvalidIndexingOp(this IMuoaValue value, IMuoaValue other) =>
        throw new RuntimeException($"Cannot index into type {value.Type()} with type {other.Type()}");

    internal static void ExpectSignature(this IMuoaFunction func, int input, int output)
    {
        (int fin, int fout) = func.Signature();

        if (input != fin || output != fout)
            throw new RuntimeException($"Expected function with sigature {input}.{output}, but found signature {fin}.{fout}");
    }
    
    internal static bool DefaultValueEquals(IMuoaValue? other) => other switch
    {
        null => throw new UnreachableException("null should not be passed to IMuoaValue::Equals"),
        _ => false
    };
}
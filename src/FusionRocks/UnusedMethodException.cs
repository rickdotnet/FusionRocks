namespace FusionRocks;

public class UnusedMethodException() : Exception("This method is not used by FusionCache")
{
    public static void Throw()
    {
        throw new UnusedMethodException();
    }
}
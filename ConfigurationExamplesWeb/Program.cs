using Microsoft.Extensions.Options;

namespace ConfigurationExamplesWeb;

public class Program
{
    public static void Main(string[] args)
    {
        Examples.WorkWithConfig(args);
        //Examples.WorkWithOptions(args);
    }
}
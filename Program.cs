using Muoa.Parsing;
using Muoa.Tokenizing;
using McMaster.Extensions.CommandLineUtils;
using Muoa.Runtime;

namespace Muoa;

internal class Program
{
    private const string VERSION = "0.5.3";
    
    public static int Main(string[] args) =>
        CommandLineApplication.Execute<Program>(args);
    
    [Option(LongName = "version", ShortName = "v", Description = "Show the current Muoa version")]
    private bool ShowVersion { get; } = false;

    [Option(LongName = "file", ShortName = "f", Description = "The file to execute")]
    private string? Input { get; } = null;

    [Option(LongName = "tokens", ShortName = "t", Description = "Show the generated lexer tokens")]
    private bool ShowTokens { get; } = false;

    [Option(LongName = "nodes", ShortName = "n", Description = "Show the generated parser nodes")]
    private bool ShowNodes { get; } = false;
    
    private int OnExecute()
    {
        if (ShowVersion)
        {
            Console.WriteLine($"Muoa Version {Program.VERSION}");
            return 0;
        }
        
        if (Input == null)
        {
            Console.Error.WriteLine("No input file");
            return 1;
        }
        
        string text;
        try
        {
            text = File.ReadAllText(Input);
        }
        catch (IOException e)
        {
            Console.Error.WriteLine($"File '{Input}' could not be read: {e.Message}");
            return 1;
        }

        try
        {
            Lexer lexer = new(text, Input);
            var tokens = lexer.Lex();

            if (ShowTokens)
            {
                foreach (Lexer.Token token in tokens)
                    Console.WriteLine(token);
            }

            Parser parser = new(tokens);

            var nodes = parser.Parse();

            if (ShowNodes)
            {
                foreach (INode node in nodes)
                    Console.WriteLine(node);
            }

            Scope scope = new(null, false);

            Stdlib.AddToScope(scope);

            Interpreter interpreter = new(scope);

            var result = interpreter.Eval(nodes);

            for (int i = result.Length - 1; i >= 0; i--)
                Console.WriteLine(result[i]);
        }
        catch (MuoaException e)
        {
            Console.Error.WriteLine(e.Message);
            return 1;
        }

        return 0;
    }
}

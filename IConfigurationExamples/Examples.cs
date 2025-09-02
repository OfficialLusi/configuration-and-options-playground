/*  

IConfiguration is the central abstraction in .NET for reading settings (key/value) from multiple providers:  
JSON files (e.g., appsettings.json), environment variables, command-line arguments, Azure Key Vault, user secrets, etc.  
Keys are case-insensitive and hierarchical (sections).  

Main components:  
- IConfiguration: the main interface for reading values/sections.  
- IConfigurationRoot: the concrete implementation resulting from the build.  
- IConfigurationBuilder: composes the providers (order = precedence).  
- IConfigurationSection: represents a section (sub-tree) of the configuration (e.g., "ConnectionStrings").  

Precedence (typical order):  
1. appsettings.json  
2. appsettings.{Environment}.json (e.g., Development)  
3. User Secrets (only in development)  
4. Environment variables  
5. Command-line arguments  
The latter overwrite the former.  

*/

using Microsoft.Extensions.Configuration;

namespace ConfigurationExamples;

public class Examples
{
    public static void WorkWithConfig(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        //.AddEnvironmentVariables()
        //.AddCommandLine(args);

        IConfiguration config = builder.Build();

        string appName = config["AppName"];
        int port = Convert.ToInt32(config["Kestrel:Endpoints:Http:UrlPort"]);

        var dbSection = config.GetSection("Database");
        string conn = dbSection["ConnectionString"];

        Console.WriteLine($"{appName} listening on port {port}, conn={conn}");
    }
}

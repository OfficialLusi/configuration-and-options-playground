/*

IConfiguration è l'astrazione centrale di .net per leggere impostazioni (chiave/valore) da più provider:
file JSON (es. appsettings.json), variabili d'ambiente, riga di comando, Azure Jey Vault, segreti d'utente, ecc.
Le chiavi sono case-insensitive e gerarchiche (sezioni).

Componenti principali:
- IConfiguration: interfaccia principale per leggere valori/sezioni.
- IConfigurationRoot: implementazione concreta risultante dal build.
- IConfigurationBuilder: compone i provider (ordine = precedenza).
- IConfigurationSection: rappresenta una sezione (sotto-albero) della configurazione (es. "ConnectionStrings").

Precedenza (ordine tipico):
1. appsettings.json
2. appsettings.{Environment}.json (es. Development)
3. User Secrets (solo in sviluppo)
4. Variabili d'ambiente
5. Argomenti della riga di comando
Gli ultimi sovrascrivono i primi.

*/

using Microsoft.Extensions.Configuration;

namespace IConfigurationExamples;

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

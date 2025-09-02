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

using Microsoft.Extensions.Options;

namespace ConfigurationExamplesWeb;

public class Examples
{
    // using only IConfiguration
    public static void WorkWithConfig(string[] args)
    {
        // builder.Configuration is already set up with default providers:
        // appsettings.json, appsettings.{Environment}.json, UserSecrets (se Development), Env vars, Command line. (priority from last to first)
        var builder = WebApplication.CreateBuilder(args);

        var app = builder.Build();

        // the object is re-binded with every request -> values are always "new", changed or not
        // no possibility to validate object
        app.MapGet("/mail", (IConfiguration cfg) => cfg.GetRequiredSection("Mail").Get<MailSettings>());

        app.Run();
    }

    // using IOptions<T> 
    public static void WorkWithOptions(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // builder.Configuration is already set up with default providers:
        // appsettings.json, appsettings.{Environment}.json, UserSecrets (se Development), Env vars, Command line.

        // to set only the wanted providers, we need to clear the defaults and then reload the wanted ones:
        builder.Configuration.Sources.Clear();
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        // the binding happen only one time, private cache and auto reload when the file change
        // possibility of validation
        builder.Services.AddOptions<MailSettings>()
            // bind the configuration only here, before the build of the builder
            .Bind(builder.Configuration.GetSection("Mail"))
            // possible to add a ValidateOnStart, the validation will be done here and not on the first access to the configuration
            //.ValidateOnStart()
            // validate class properties that have data annotations (es. [Required], [EmailAddress], [Range(1,65535)], ecc.)
            .ValidateDataAnnotations()
            // custom validation
            .Validate(o => o.Port > 0, "Port must be > 0");

        // possibility to build a IvalidateOptions<T> for deeper validation logic
        builder.Services.AddSingleton<IValidateOptions<MailSettings>, CustomValidation>();

        var app = builder.Build();

        // Endpoint to show the bound settings, with reload on change
        app.MapGet("/mail", (IOptionsMonitor<MailSettings> mon) => mon.CurrentValue);
        app.Run();
    }



    #region MailSettingsClass
    public class MailSettings
    {
        public string Host { get; init; } = "";
        public int Port { get; init; }
        public string From { get; init; } = "";
    }
    #endregion
}

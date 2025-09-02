using Microsoft.Extensions.Options;
using static ConfigurationExamplesWeb.Examples;

namespace ConfigurationExamplesWeb
{
    public class CustomValidation : IValidateOptions<MailSettings>
    {
        public ValidateOptionsResult Validate(string? name, MailSettings options)
        {
            if (string.IsNullOrEmpty(options.Host))
                return ValidateOptionsResult.Fail("Host is required.");
            if (options.Port <= 0 || options.Port > 65535)
                return ValidateOptionsResult.Fail("Port must be between 1 and 65535.");
            if (string.IsNullOrEmpty(options.From))
                return ValidateOptionsResult.Fail("From address is required.");

            // Add more custom validations as needed
            return ValidateOptionsResult.Success;
        }
    }
}

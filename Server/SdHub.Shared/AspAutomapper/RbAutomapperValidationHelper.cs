using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SdHub.Shared.AspAutomapper;

public class RbAutomapperValidationHelper
{
    public static void ValidateAutomapper(IServiceProvider serviceProvider)
    {
        var environment = serviceProvider.GetRequiredService<IHostEnvironment>();
        var logger = serviceProvider.GetRequiredService<ILogger<RbAutomapperValidationHelper>>();
        if (!environment.IsDevelopment())
        {
            logger.LogInformation("Skip Automapper check in non Development env");
            return;
        }

        var scope = serviceProvider.CreateScope();
        try
        {
            var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
            mapper.ConfigurationProvider.AssertConfigurationIsValid();
            logger.LogInformation("Automapper profiles is valid");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during automapper validation");
            throw;
        }
    }
}

using Reinforced.Typings.Fluent;

namespace SdHub.TsConfigurators.Shared;

public interface ITsConfigurator
{
    const string ModelsRoot = "../../Client/apps/SdHub/src/app/models/autogen";
    void Configure(ConfigurationBuilder builder);
}
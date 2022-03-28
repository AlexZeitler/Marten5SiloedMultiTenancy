using Marten;
using Weasel.Core;

namespace Marten5SiloedMultiTenancy.Config;

public class MartenConfiguration
{
  private const string DefaultSchema = "public";

  public string ConnectionString { get; set; } = default!;

  public bool ShouldRecreateDatabase { get; set; } = false;

  public bool UseMetadata = true;
}

public static class MartenConfigurationExtensions
{
  private const string DefaultConfigKey = "EventStore";

  public static IServiceCollection AddMartenEventStore(
    this IServiceCollection services,
    IConfiguration config,
    Action<StoreOptions>? configureOptions = null,
    string configKey = DefaultConfigKey
  )
  {
    var martenConfig = config.GetSection(configKey)
      .Get<MartenConfiguration>();

    services.AddMarten(_ => SetStoreOptions(martenConfig, configureOptions));

    return services;
  }


  private static StoreOptions SetStoreOptions(
    MartenConfiguration config,
    Action<StoreOptions>? configureOptions = null
  )
  {
    var options = new StoreOptions();
    options.MultiTenantedWithSingleServer(config.ConnectionString);
    options.AutoCreateSchemaObjects = AutoCreate.All;

    options.UseDefaultSerialization(
      EnumStorage.AsString,
      nonPublicMembersStorage: NonPublicMembersStorage.All
    );

    configureOptions?.Invoke(options);

    return options;
  }
}
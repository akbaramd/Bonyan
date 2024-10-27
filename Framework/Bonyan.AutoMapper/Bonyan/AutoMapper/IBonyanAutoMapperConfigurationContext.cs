using AutoMapper;

namespace Bonyan.AutoMapper;

public interface IBonyanAutoMapperConfigurationContext
{
  IMapperConfigurationExpression MapperConfiguration { get; }

  IServiceProvider ServiceProvider { get; }
}


public class BonyanAutoMapperConfigurationContext : IBonyanAutoMapperConfigurationContext
{
  public IMapperConfigurationExpression MapperConfiguration { get; }

  public IServiceProvider ServiceProvider { get; }

  public BonyanAutoMapperConfigurationContext(
    IMapperConfigurationExpression mapperConfigurationExpression,
    IServiceProvider serviceProvider)
  {
    MapperConfiguration = mapperConfigurationExpression;
    ServiceProvider = serviceProvider;
  }
}


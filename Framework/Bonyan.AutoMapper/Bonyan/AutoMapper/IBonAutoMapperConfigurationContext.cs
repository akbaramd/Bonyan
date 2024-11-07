using AutoMapper;

namespace Bonyan.AutoMapper;

public interface IBonAutoMapperConfigurationContext
{
  IMapperConfigurationExpression MapperConfiguration { get; }

  IServiceProvider ServiceProvider { get; }
}


public class BonAutoMapperConfigurationContext : IBonAutoMapperConfigurationContext
{
  public IMapperConfigurationExpression MapperConfiguration { get; }

  public IServiceProvider ServiceProvider { get; }

  public BonAutoMapperConfigurationContext(
    IMapperConfigurationExpression mapperConfigurationExpression,
    IServiceProvider serviceProvider)
  {
    MapperConfiguration = mapperConfigurationExpression;
    ServiceProvider = serviceProvider;
  }
}


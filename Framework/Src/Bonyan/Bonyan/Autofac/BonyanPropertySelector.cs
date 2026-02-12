using System.Reflection;
using Autofac.Core;
using Bonyan.DependencyInjection;

namespace Bonyan.Autofac;

public class BonyanPropertySelector : DefaultPropertySelector
{
  public BonyanPropertySelector(bool preserveSetValues)
    : base(preserveSetValues)
  {
  }

  public override bool InjectProperty(PropertyInfo propertyInfo, object instance)
  {
    return propertyInfo.GetCustomAttributes(typeof(DisablePropertyInjectionAttribute), true).IsNullOrEmpty() &&
           base.InjectProperty(propertyInfo, instance);
  }
}

using Bonyan.Core;
using Bonyan.Data;
using JetBrains.Annotations;

namespace Bonyan.MultiTenant;

[Serializable]
public class BonTenantConfiguration
{
  public Guid Id { get; set; }

  public string Name { get; set; } = default!;

  public ConnectionStrings? ConnectionStrings { get; set; }

  public bool IsActive { get; set; }

  public BonTenantConfiguration()
  {
    IsActive = true;
  }

  public BonTenantConfiguration(Guid id, [NotNull] string name)
    : this()
  {
    Check.NotNull(name, nameof(name));

    Id = id;
    Name = name;

    ConnectionStrings = new ConnectionStrings();
  }
}

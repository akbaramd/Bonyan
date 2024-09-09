namespace Bonyan.AspNetCore.Persistence;

public interface ISeeder
{
  Task SeedAsync(CancellationToken cancellationToken);
}

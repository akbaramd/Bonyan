namespace Bonyan.Persistence.EntityFrameworkCore;


public interface ISeeder
{

  Task SeedAsync(CancellationToken cancellationToken = default);
 
}
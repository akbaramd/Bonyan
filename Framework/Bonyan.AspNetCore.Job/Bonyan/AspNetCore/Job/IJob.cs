using Bonyan.UnitOfWork;

namespace Bonyan.AspNetCore.Job;

public  interface IJob: IUnitOfWorkEnabled
{
  Task ExecuteAsync(CancellationToken cancellationToken = default!);
}





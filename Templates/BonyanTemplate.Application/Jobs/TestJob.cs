using Bonyan.AspNetCore.Job;
using Bonyan.DomainDrivenDesign.Domain.Abstractions;
using BonyanTemplate.Domain.Entities;
using BonyanTemplate.Domain.Repositories;

namespace BonyanTemplate.Application.Jobs;

[CronJob("*/1 * * * *")]
public class TestJob : IJob
{
  // private IBooksRepository _booksRepository;
  // private IRepository<Books,Guid> _repository;
  // private IRepository<Books> _2repository;

  // public TestJob(IBooksRepository booksRepository, IRepository<Books, Guid> repository, IRepository<Books> repository1)
  // {
    // _booksRepository = booksRepository;
    // _repository = repository;
    // _2repository = repository1;
  // }

  public Task ExecuteAsync(CancellationToken cancellationToken = default)
  {
    Console.WriteLine("Tick Tok");
    return Task.CompletedTask;
  }
}

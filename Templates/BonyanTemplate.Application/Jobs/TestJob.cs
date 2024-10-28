using Bonyan.AspNetCore.Job;
using Bonyan.DomainDrivenDesign.Domain.Abstractions;
using Bonyan.MultiTenant;
using Bonyan.TenantManagement.Domain.Bonyan.TenantManagement.Domain;
using BonyanTemplate.Domain.Entities;
using BonyanTemplate.Domain.Repositories;

namespace BonyanTemplate.Application.Jobs;

public class TestJob : IJob
{
  private ITenantRepository _tenantRepository;
  private IRepository<Books> _repository;
  private ICurrentTenant _currentTenantAccessor;
  public TestJob(ITenantRepository tenantRepository, ICurrentTenant currentTenantAccessor, IRepository<Books> repository)
  {
    _tenantRepository = tenantRepository;
    _currentTenantAccessor = currentTenantAccessor;
    _repository = repository;
  }
  // private IRepository<Books,Guid> _repository;
  // private IRepository<Books> _2repository;

  // public TestJob(IBooksRepository booksRepository, IRepository<Books, Guid> repository, IRepository<Books> repository1)
  // {
    // _booksRepository = booksRepository;
    // _repository = repository;
    // _2repository = repository1;
  // }

  public async Task ExecuteAsync(CancellationToken cancellationToken = default)
  {
    _currentTenantAccessor.Change(id:Guid.Parse("40C84038-6B5B-492C-B257-774B2BCB2FA1"));
    var res = await _repository.FindAsync(x=>true);
    Console.WriteLine("Tick Tok");
  }
}

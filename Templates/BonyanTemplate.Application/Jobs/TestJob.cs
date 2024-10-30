using Bonyan.AspNetCore.Job;
using Bonyan.MultiTenant;
using Bonyan.TenantManagement.Application.Dto;
using Bonyan.TenantManagement.Application.Services;
using Bonyan.TenantManagement.Domain;
using Bonyan.UnitOfWork;

namespace BonyanTemplate.Application.Jobs;

public class TestJob : IJob
{
  private ITenantRepository _tenantRepository;
  private ITenantApplicationService _service;
  private ICurrentTenant _currentTenantAccessor;
  private IUnitOfWork _unitOfWork;
  public TestJob(ITenantRepository tenantRepository, ICurrentTenant currentTenantAccessor, ITenantApplicationService repository, IUnitOfWork unitOfWork)
  {
    _tenantRepository = tenantRepository;
    _currentTenantAccessor = currentTenantAccessor;
    _service = repository;
    _unitOfWork = unitOfWork;
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
    // var res = await _service.CreateAsync(new TenantCreateDto(){Key = "test"});
    Console.WriteLine("Tick Tok");
  }
}

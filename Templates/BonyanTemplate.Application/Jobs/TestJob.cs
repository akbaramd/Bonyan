using Bonyan.AspNetCore.Job;
using Bonyan.MultiTenant;
using Bonyan.TenantManagement.Application.Services;
using Bonyan.TenantManagement.Domain;
using Bonyan.UnitOfWork;
using Bonyan.UserManagement.Domain.ValueObjects;
using BonyanTemplate.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BonyanTemplate.Application.Jobs;

public class TestJob : IJob
{
  
  private UserManager<User> _userManager;
  private IUnitOfWorkManager _unitOfWorkManager;

  public TestJob( UserManager<User> userManage, IUnitOfWorkManager unitOfWorkManager)
  {
    _unitOfWorkManager = unitOfWorkManager;
    _userManager = userManage;
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
    using (var uow  = _unitOfWorkManager.Begin())
    {
      var res = await _userManager.CreateAsync(new User(new UserId(),"akbarsafari00"), "Aa@13567975");

      await uow.CompleteAsync();
    }
    // var res = await _service.CreateAsync(new TenantCreateDto(){Key = "test"});
    
    
    
    Console.WriteLine("Tick Tok");
  }
}

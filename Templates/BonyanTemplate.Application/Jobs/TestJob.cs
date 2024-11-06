using Bonyan.AspNetCore.Job;
using Bonyan.UnitOfWork;
using Bonyan.UserManagement.Domain.Repositories;
using Bonyan.UserManagement.Domain.ValueObjects;
using BonyanTemplate.Domain.Entities;
using BonyanTemplate.Domain.Enums;
using BonyanTemplate.Domain.Repositories;
using Microsoft.AspNetCore.Identity;

namespace BonyanTemplate.Application.Jobs;

public class TestJob : IJob
{

  private IBooksRepository _userRepository;
  private IAuthorsRepository _authorsRepository;
  private IUnitOfWorkManager _unitOfWorkManager;
  public TestJob(IBooksRepository userRepository, IAuthorsRepository authorsRepository, IUnitOfWorkManager unitOfWorkManager)
  {
    _userRepository = userRepository;
    _authorsRepository = authorsRepository;
    _unitOfWorkManager = unitOfWorkManager;
  }


  
  public async Task ExecuteAsync(CancellationToken cancellationToken = default)
  {

      var auth = await _authorsRepository.FindOneAsync(x => x.Title.Equals("asd"));
      if (auth == null)
      {
        auth = new Authors() { Id = AuthorId.CreateNew(),Title = "asd" };
        await _authorsRepository.AddAsync(auth, true);
      }
    
      var res = await _userRepository.AddAsync(new Books()
      {
        Id = BookId.CreateNew(),
        Title = "",
        Status = BookStatus.Available,
        Author = auth,
        AuthorId = auth.Id
      },true);

      var s = await _userRepository.FindOneAsync(x => x.Id == res.Id);
      
      
      s.Status = BookStatus.OutOfStock;
      await _userRepository.UpdateAsync(s,true);

    
    
    Console.WriteLine("Tick Tok");
  }
}

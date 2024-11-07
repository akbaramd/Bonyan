using Bonyan.AspNetCore.Job;
using Bonyan.UnitOfWork;
using Bonyan.UserManagement.Domain.Repositories;
using Bonyan.UserManagement.Domain.ValueObjects;
using BonyanTemplate.Domain.Entities;
using BonyanTemplate.Domain.Enums;
using BonyanTemplate.Domain.Repositories;
using Microsoft.AspNetCore.Identity;

namespace BonyanTemplate.Application.Jobs;

public class TestBonWorker : IBonWorker
{

  private IBooksBonRepository _userBonRepository;
  private IAuthorsBonRepository _authorsBonRepository;
  private IBonUnitOfWorkManager _bonUnitOfWorkManager;
  public TestBonWorker(IBooksBonRepository userBonRepository, IAuthorsBonRepository authorsBonRepository, IBonUnitOfWorkManager bonUnitOfWorkManager)
  {
    _userBonRepository = userBonRepository;
    _authorsBonRepository = authorsBonRepository;
    _bonUnitOfWorkManager = bonUnitOfWorkManager;
  }


  
  [BonUnitOfWork]
  public async Task ExecuteAsync(CancellationToken cancellationToken = default)
  {

      var auth = await _authorsBonRepository.FindOneAsync(x => x.Title.Equals("asd"));
      if (auth == null)
      {
        auth = new Authors() { Id = AuthorId.CreateNew(),Title = "asd" };
        await _authorsBonRepository.AddAsync(auth, true);
      }
    
      var res = await _userBonRepository.AddAsync(new Books()
      {
        Id = BookId.CreateNew(),
        Title = "",
        Status = BookStatus.Available,
        Author = auth,
        AuthorId = auth.Id
      },true);

      var s = await _userBonRepository.FindOneAsync(x => x.Id == res.Id);
      
      
      s.Status = BookStatus.OutOfStock;
      await _userBonRepository.UpdateAsync(s,true);

    
    
    Console.WriteLine("Tick Tok");
  }
}

using Bonyan.AspNetCore.Job;
using Bonyan.UnitOfWork;
using Bonyan.UserManagement.Domain.ValueObjects;
using BonyanTemplate.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace BonyanTemplate.Application.Jobs;

public class TestJob : IJob
{
  





  public async Task ExecuteAsync(CancellationToken cancellationToken = default)
  {
      // var res = await _userManager.CreateAsync(new User(new UserId(),"akbarsafari00"), "Aa@13567975");
    
    Console.WriteLine("Tick Tok");
  }
}

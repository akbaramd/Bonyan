using Bonyan.AspNetCore.Persistence;
using Bonyan.DomainDrivenDesign.Domain.Abstractions;
using BonyanTemplate.Domain.Entities;

namespace BonyanTemplate.Infrastructure.Seeds;

public class BookSeed : ISeeder
{
    private readonly IRepository<Books, Guid> _repository;

    public BookSeed(IRepository<Books, Guid> repository)
    {
        _repository = repository;
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        await _repository.AddAsync(new Books() { Title = "ss" });
    }
}
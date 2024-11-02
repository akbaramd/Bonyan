using Bonyan.Layer.Domain.Abstractions;
using BonyanTemplate.Domain.Entities;

namespace BonyanTemplate.Domain.Repositories;

public interface IAuthorsRepository : IRepository<Authors, AuthorId>
{
}
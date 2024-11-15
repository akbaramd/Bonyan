using Bonyan.Layer.Domain.Repository.Abstractions;
using BonyanTemplate.Domain.Entities;

namespace BonyanTemplate.Domain.Repositories;

public interface IAuthorsBonRepository : IBonRepository<Authors, AuthorId>
{
}
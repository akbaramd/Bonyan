using Bonyan.Layer.Domain.Repository.Abstractions;

namespace BonyanTemplate.Domain.Authors;

/// <summary>
/// Repository for <see cref="Author"/> aggregate root.
/// </summary>
public interface IAuthorsBonRepository : IBonRepository<Author, AuthorId>
{
}
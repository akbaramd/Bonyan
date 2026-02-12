using Bonyan.Layer.Application.Services;
using BonyanTemplate.Application.Authors.Dtos;
using BonyanTemplate.Domain.Authors;

namespace BonyanTemplate.Application.Authors;

/// <summary>
/// Default implementation of <see cref="IAuthorAppService"/>.
/// </summary>
public class AuthorAppService : BonCrudAppService<Author, AuthorId, AuthorDto>, IAuthorAppService
{
}
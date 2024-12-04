using Bonyan.Layer.Application.Services;
using Bonyan.Layer.Domain.Repository.Abstractions;
using BonyanTemplate.Application.Authors.Dtos;
using BonyanTemplate.Domain.Authors;

namespace BonyanTemplate.Application.Authors;

public class AuthorAppService : BonCrudAppService<Domain.Authors.Authors,AuthorId,AuthorDto>,IAuthorAppService
{
   
}
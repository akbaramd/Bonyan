using Bonyan.Layer.Application.Dto;
using Bonyan.Layer.Application.Services;
using BonyanTemplate.Application.Authors.Dtos;
using BonyanTemplate.Domain.Authors;

namespace BonyanTemplate.Application.Authors;

public interface IAuthorAppService : IBonCrudAppService<AuthorId,BonPaginateDto,AuthorDto>
{
    
}
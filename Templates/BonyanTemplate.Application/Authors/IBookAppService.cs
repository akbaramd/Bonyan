using Bonyan.Layer.Application.Abstractions;
using Bonyan.Layer.Application.Dto;
using BonyanTemplate.Application.Authors.Dtos;
using BonyanTemplate.Domain.Authors;

namespace BonyanTemplate.Application.Authors;

public interface IAuthorAppService : ICrudAppService<AuthorId,BonPaginateDto,AuthorDto>
{
    
}
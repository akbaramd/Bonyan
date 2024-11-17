using Bonyan.Layer.Application.Dto;
using BonyanTemplate.Domain.Authors;

namespace BonyanTemplate.Application.Authors.Dtos;

public class AuthorDto : BonAggregateRootDto<AuthorId>
{
    public string Title { get; set; }   
}


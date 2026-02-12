using AutoMapper;
using BonyanTemplate.Application.Authors.Dtos;
using BonyanTemplate.Domain.Authors;

namespace BonyanTemplate.Application.Authors;

public class AuthorMapper : Profile
{
    public AuthorMapper()
    {
        CreateMap<AuthorDto, Author>().ReverseMap();
    }
}
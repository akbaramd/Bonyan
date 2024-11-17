using AutoMapper;
using BonyanTemplate.Application.Authors.Dtos;
using BonyanTemplate.Application.Books.Dtos;

namespace BonyanTemplate.Application.Authors;

public class AuthorMapper : Profile
{
    public AuthorMapper()
    {
        CreateMap<AuthorDto,Domain.Authors.Authors>().ReverseMap();
    }
}
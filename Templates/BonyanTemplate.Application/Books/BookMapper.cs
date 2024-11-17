using AutoMapper;
using BonyanTemplate.Application.Books.Dtos;
using BonyanTemplate.Domain.Books;

namespace BonyanTemplate.Application.Books;

public class BookMapper : Profile
{
    public BookMapper()
    {
        CreateMap<BookDto, Book>().ReverseMap();
    }
}
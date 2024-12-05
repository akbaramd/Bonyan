﻿using Bonyan.Layer.Application.Services;
using BonyanTemplate.Application.Books.Dtos;
using BonyanTemplate.Domain.Books;

namespace BonyanTemplate.Application.Books;

public class BookAppService : BonCrudAppService<Book,BookId,BookDto>,IBookAppService
{
   
}
﻿using Bonyan.DomainDrivenDesign.Domain.Abstractions;
using BonyanTemplate.Domain.Entities;

namespace BonyanTemplate.Domain.Repositories;

public interface  IBooksRepository : IRepository<Books,Guid>
{
      
}
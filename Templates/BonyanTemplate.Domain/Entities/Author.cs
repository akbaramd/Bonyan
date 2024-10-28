﻿using Bonyan.Layer.Domain.Aggregates;
using Bonyan.Layer.Domain.ValueObjects;
using BonyanTemplate.Domain.DomainEvents;
using BonyanTemplate.Domain.Enums;

namespace BonyanTemplate.Domain.Entities;

public class Authors : TenantAggregateRoot<AuthorId>
{
  public string Title { get; set; } = string.Empty;

  public Authors()
  {
    AddDomainEvent(new BookCreated());
  }
}


public class AuthorId : BusinessId<AuthorId>{}
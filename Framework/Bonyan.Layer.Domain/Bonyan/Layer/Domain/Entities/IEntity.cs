﻿namespace Bonyan.Layer.Domain.Entities;

public interface IEntity
{
  object[] GetKeys();
}

public interface IEntity<TKey> : IEntity
{
  public TKey Id { get; set; }
}
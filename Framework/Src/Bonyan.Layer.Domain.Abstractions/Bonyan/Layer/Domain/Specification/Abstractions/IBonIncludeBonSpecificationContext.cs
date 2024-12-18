﻿using Microsoft.EntityFrameworkCore.Query;

namespace Bonyan.Layer.Domain.Specification.Abstractions;

public interface IBonIncludeBonSpecificationContext<T, TProperty> : IBonSpecificationContext<T> where T : class
{
    new IIncludableQueryable<T,TProperty> Query { get; }
}
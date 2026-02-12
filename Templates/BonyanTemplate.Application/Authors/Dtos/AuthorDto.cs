using Bonyan.Layer.Application.Dto;
using BonyanTemplate.Domain.Authors;

namespace BonyanTemplate.Application.Authors.Dtos;

/// <summary>
/// Data transfer object for <see cref="Author"/>.
/// </summary>
public class AuthorDto : BonAggregateRootDto<AuthorId>
{
    /// <summary>Author display name or title.</summary>
    public string Title { get; set; } = string.Empty;
}


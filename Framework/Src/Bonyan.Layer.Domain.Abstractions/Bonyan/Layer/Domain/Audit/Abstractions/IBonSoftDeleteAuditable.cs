namespace Bonyan.Layer.Domain.Audit.Abstractions;

public interface IBonSoftDeleteAuditable
{
    bool IsDeleted { get; set;}
    DateTime? DeletedAt { get; set;}
}
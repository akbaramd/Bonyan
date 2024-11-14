namespace Bonyan.Layer.Domain.Audit.Abstractions;

public interface IBonSoftDeleteAuditable
{
    bool IsDeleted { get; set;}
    DateTime? DeletedDate { get; set;}
}
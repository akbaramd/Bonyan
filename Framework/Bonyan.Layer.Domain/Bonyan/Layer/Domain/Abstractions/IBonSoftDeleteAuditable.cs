namespace Bonyan.Layer.Domain.Abstractions;

public interface IBonSoftDeleteAuditable
{
    bool IsDeleted { get; }
    DateTime? DeletedDate { get; set; }
}
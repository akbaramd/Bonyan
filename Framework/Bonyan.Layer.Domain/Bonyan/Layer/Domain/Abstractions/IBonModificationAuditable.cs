namespace Bonyan.Layer.Domain.Abstractions;

public interface IBonModificationAuditable
{
    DateTime? ModifiedDate { get; set; }
}
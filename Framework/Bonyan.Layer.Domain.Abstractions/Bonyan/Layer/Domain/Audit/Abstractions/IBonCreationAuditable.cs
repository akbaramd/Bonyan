namespace Bonyan.Layer.Domain.Audit.Abstractions;

public interface IBonCreationAuditable
{
    DateTime CreatedDate { get; set; }
}
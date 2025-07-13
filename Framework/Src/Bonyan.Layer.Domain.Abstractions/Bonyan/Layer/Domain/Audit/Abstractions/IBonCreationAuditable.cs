namespace Bonyan.Layer.Domain.Audit.Abstractions;

public interface IBonCreationAuditable
{
    DateTime CreatedAt { get; set; }
}
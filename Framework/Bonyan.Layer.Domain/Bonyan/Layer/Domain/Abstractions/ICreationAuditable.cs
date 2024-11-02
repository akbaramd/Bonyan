namespace Bonyan.Layer.Domain.Abstractions;

public interface ICreationAuditable
{
    DateTime CreatedDate { get; set; }
}
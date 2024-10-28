namespace Bonyan.Layer.Domain.Abstractions;

public interface IModificationAuditable
{
  DateTime? ModifiedDate { get; set; }
}
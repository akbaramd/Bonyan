namespace Bonyan.DDD.Domain.Abstractions;

public interface ICreationAuditable
{
  DateTime CreatedDate { get; set; }
}

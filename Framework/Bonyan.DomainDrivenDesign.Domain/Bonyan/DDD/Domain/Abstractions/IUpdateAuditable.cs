namespace Bonyan.DDD.Domain.Abstractions;

public interface IUpdateAuditable : ICreationAuditable
{
  DateTime? UpdatedDate { get; set; }
}

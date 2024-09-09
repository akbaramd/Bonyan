namespace Bonyan.DomainDrivenDesign.Domain.Abstractions;

public interface IUpdateAuditable : ICreationAuditable
{
  DateTime? UpdatedDate { get; set; }
}

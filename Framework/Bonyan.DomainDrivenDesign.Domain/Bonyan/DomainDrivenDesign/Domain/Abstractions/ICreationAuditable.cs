namespace Bonyan.DomainDrivenDesign.Domain.Abstractions;

public interface ICreationAuditable
{
  DateTime CreatedDate { get; set; }
}

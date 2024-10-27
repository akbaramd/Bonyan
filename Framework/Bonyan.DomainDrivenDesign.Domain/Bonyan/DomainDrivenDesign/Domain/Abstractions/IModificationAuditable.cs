namespace Bonyan.DomainDrivenDesign.Domain.Abstractions;

public interface IModificationAuditable
{
  DateTime? ModifiedDate { get; set; }
}
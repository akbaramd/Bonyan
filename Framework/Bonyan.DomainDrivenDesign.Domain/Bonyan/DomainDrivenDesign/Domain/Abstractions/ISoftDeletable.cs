namespace Bonyan.DomainDrivenDesign.Domain.Abstractions;

public interface ISoftDeletable
{
  bool IsDeleted { get; }
  DateTime? DeletedDate { get; set; }
}

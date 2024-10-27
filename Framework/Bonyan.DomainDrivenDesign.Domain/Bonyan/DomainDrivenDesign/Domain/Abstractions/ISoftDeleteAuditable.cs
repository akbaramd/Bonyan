namespace Bonyan.DomainDrivenDesign.Domain.Abstractions;

public interface ISoftDeleteAuditable
{
  bool IsDeleted { get; }
  DateTime? DeletedDate { get; set; }
}

namespace Bonyan.Layer.Domain.Audit.Abstractions;

public interface IBonModificationAuditable
{
    DateTime? ModifiedDate { get;set;  }
}
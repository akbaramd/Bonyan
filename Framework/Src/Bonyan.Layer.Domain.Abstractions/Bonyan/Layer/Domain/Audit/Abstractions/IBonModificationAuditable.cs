namespace Bonyan.Layer.Domain.Audit.Abstractions;

public interface IBonModificationAuditable
{
    DateTime? ModifiedAt { get;set;  }
}
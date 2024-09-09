namespace Bonyan.DomainDrivenDesign.Domain.Abstractions;

public interface IFullAuditable : ICreationAuditable, IUpdateAuditable, ISoftDeletable
{
}

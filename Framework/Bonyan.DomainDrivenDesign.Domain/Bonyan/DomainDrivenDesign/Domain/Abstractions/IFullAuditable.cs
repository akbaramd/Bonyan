namespace Bonyan.DomainDrivenDesign.Domain.Abstractions;

public interface IFullAuditable : ICreationAuditable, IModificationAuditable, ISoftDeleteAuditable
{
}

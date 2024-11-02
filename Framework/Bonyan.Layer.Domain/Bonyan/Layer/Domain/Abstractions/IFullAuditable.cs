namespace Bonyan.Layer.Domain.Abstractions;

public interface IFullAuditable : ICreationAuditable, IModificationAuditable, ISoftDeleteAuditable
{
}
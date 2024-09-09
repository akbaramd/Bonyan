namespace Bonyan.DDD.Domain.Abstractions;

public interface IFullAuditable : ICreationAuditable, IUpdateAuditable, ISoftDeletable
{
}

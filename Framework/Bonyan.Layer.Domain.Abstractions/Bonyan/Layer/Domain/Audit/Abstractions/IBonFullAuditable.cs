namespace Bonyan.Layer.Domain.Audit.Abstractions;

public interface IBonFullAuditable : IBonCreationAuditable, IBonModificationAuditable, IBonSoftDeleteAuditable
{
}
using Bonyan.Layer.Application.Dto;
using Bonyan.Layer.Application.Exceptions;
using Bonyan.Layer.Domain.Entity;
using Bonyan.Layer.Domain.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.Layer.Application.Services;

public abstract class AbstractCrudAppService<TEntity, TKey, TPaginateDto, TGetOutputDto, TGetListOutputDto,
    TCreateInput, TUpdateInput>
    : AbstractBonReadonlyAppService<TEntity, TKey, TPaginateDto, TGetOutputDto, TGetListOutputDto>,
        IBonCrudAppService<TKey, TPaginateDto, TCreateInput, TUpdateInput, TGetOutputDto, TGetListOutputDto>
    where TEntity : class, IBonEntity<TKey>
    where TGetListOutputDto : IBonEntityDto<TKey>
    where TGetOutputDto : IBonEntityDto<TKey>
    where TCreateInput : class
    where TUpdateInput : class
    where TPaginateDto : IBonPaginateDto
{
    protected new IBonRepository<TEntity, TKey> Repository { get; }

    protected AbstractCrudAppService(IBonRepository<TEntity, TKey> repository) : base(repository)
    {
        Repository = repository;
    }

    #region Create

    public virtual async Task<ServiceResult<TGetOutputDto>> CreateAsync(TCreateInput input)
    {
        try
        {
            var entity = MapCreateDtoToEntity(input);
            await Repository.AddAsync(entity);
            await UnitOfWorkManager.Current?.SaveChangesAsync();

            return ServiceResult<TGetOutputDto>.Success(MapToDto(entity));
        }
        catch (Exception ex)
        {
            return ServiceResult<TGetOutputDto>.Failure($"Error creating entity: {ex.Message}");
        }
    }

    #endregion

    #region Update

    public virtual async Task<ServiceResult<TGetOutputDto>> UpdateAsync(TKey id, TUpdateInput input)
    {
        try
        {
            var entity = await GetEntityByIdAsync(id);
            MapUpdateDtoToEntity(input, entity);
            await Repository.UpdateAsync(entity);
            await UnitOfWorkManager.Current?.SaveChangesAsync();

            return ServiceResult<TGetOutputDto>.Success(MapToDto(entity));
        }
        catch (BonEntityNotFoundException ex)
        {
            return ServiceResult<TGetOutputDto>.Failure($"Entity not found for update: {ex.Message}");
        }
        catch (Exception ex)
        {
            return ServiceResult<TGetOutputDto>.Failure($"Error updating entity: {ex.Message}");
        }
    }

    #endregion

    #region Delete

    public virtual async Task<ServiceResult> DeleteAsync(TKey id)
    {
        try
        {
            var entity = await GetEntityByIdAsync(id);
            await Repository.DeleteAsync(entity);
            await UnitOfWorkManager.Current?.SaveChangesAsync();

            return ServiceResult.Success();
        }
        catch (BonEntityNotFoundException ex)
        {
            return ServiceResult.Failure($"Entity not found for deletion: {ex.Message}");
        }
        catch (Exception ex)
        {
            return ServiceResult.Failure($"Error deleting entity: {ex.Message}");
        }
    }

    #endregion

    #region Mapping Helpers

    protected virtual TEntity MapCreateDtoToEntity(TCreateInput input)
    {
        return Mapper.Map<TCreateInput, TEntity>(input);
    }

    protected virtual void MapUpdateDtoToEntity(TUpdateInput input, TEntity entity)
    {
        Mapper.Map(input, entity);
    }

    #endregion
}
using Bonyan.AspNetCore.Job;
using Bonyan.Messaging.Abstractions;
using Bonyan.UnitOfWork;
using BonyanTemplate.Domain.DomainEvents;
using BonyanTemplate.Domain.Entities;
using BonyanTemplate.Domain.Enums;
using BonyanTemplate.Domain.Repositories;

namespace BonyanTemplate.Application.Jobs;

public class TestBonWorker : IBonWorker
{
    private IBooksBonRepository _userBonRepository;
    private IAuthorsBonRepository _authorsBonRepository;
    private IBonUnitOfWorkManager _bonUnitOfWorkManager;
    private IBonMessageDispatcher _messageDispatcher;

    public TestBonWorker(IBooksBonRepository userBonRepository, IAuthorsBonRepository authorsBonRepository,
        IBonUnitOfWorkManager bonUnitOfWorkManager, IBonMessageDispatcher messageDispatcher)
    {
        _userBonRepository = userBonRepository;
        _authorsBonRepository = authorsBonRepository;
        _bonUnitOfWorkManager = bonUnitOfWorkManager;
        _messageDispatcher = messageDispatcher;
    }


    [BonUnitOfWork]
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        await _messageDispatcher.PublishAsync(new BookCreated(), cancellationToken);
    }
}
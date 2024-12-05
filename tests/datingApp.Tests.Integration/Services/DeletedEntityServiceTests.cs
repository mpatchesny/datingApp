using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Infrastructure;
using datingApp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace datingApp.Tests.Integration.Services;

public class DeletedEntityServiceTests : IDisposable
{
    [Fact]
    public async void add_adds_entity_to_database()
    {
        var service = new DeletedEntityService(_dbContext);
        var deletedEntity = Guid.NewGuid();

        await service.AddAsync(deletedEntity);
        _dbContext.ChangeTracker.Clear();

        var retrievedDeletedEntity = await _dbContext.DeletedEntities.FirstOrDefaultAsync(x => x.Id == deletedEntity);
        Assert.Equal(deletedEntity, retrievedDeletedEntity.Id);
    }

    [Fact]
    public async void given_entity_already_is_in_database_add_throws_exception()
    {
        var deletedEntity = Guid.NewGuid();
        await _dbContext.DeletedEntities.AddAsync(new DeletedEntityDto() { Id = deletedEntity} );
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();

        var service = new DeletedEntityService(_dbContext);
        var exception = await Record.ExceptionAsync(async () => await service.AddAsync(deletedEntity));
        Assert.NotNull(exception);
    }

    [Fact]
    public async void given_entity_is_in_database_exists_returns_true()
    {
        var deletedEntity = Guid.NewGuid();
        await _dbContext.DeletedEntities.AddAsync(new DeletedEntityDto() { Id = deletedEntity} );
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();

        var service = new DeletedEntityService(_dbContext);
        var exists = await service.ExistsAsync(deletedEntity);
        Assert.True(exists);
    }

    [Fact]
    public async void given_entity_is_not_in_database_exists_returns_false()
    {
        var deletedEntity = Guid.NewGuid();
        await _dbContext.DeletedEntities.AddAsync(new DeletedEntityDto() { Id = deletedEntity} );
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();

        var service = new DeletedEntityService(_dbContext);
        var exists = await service.ExistsAsync(Guid.NewGuid());
        Assert.False(exists);
    }

    private readonly TestDatabase _testDb;
    private readonly DatingAppDbContext _dbContext;
    public DeletedEntityServiceTests()
    {
        _testDb = new TestDatabase();
        _dbContext = _testDb.DbContext;
    }

    public void Dispose()
    {
        _testDb.Dispose();
    }
}
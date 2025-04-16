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
    public async void add_adds_entity_id_to_database()
    {
        var deletedEntityId = Guid.NewGuid();
        await _service.AddAsync(deletedEntityId, "foo", DateTime.UtcNow);
        _dbContext.ChangeTracker.Clear();

        var retrievedDeletedEntity = await _dbContext.DeletedEntities.FirstOrDefaultAsync(x => x.Id == deletedEntityId);
        Assert.Equal(deletedEntityId, retrievedDeletedEntity.Id);
    }

    [Fact]
    public async void given_entity_id_is_already_in_database_add_throws_exception()
    {
        var deletedEntityId = Guid.NewGuid();
        var entityType = "foo";
        var deletedAt = DateTime.UtcNow;
        await _dbContext.DeletedEntities.AddAsync(new DeletedEntityDto() { Id = deletedEntityId, EntityType = entityType, DeletedAt = deletedAt } );
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();

        var exception = await Record.ExceptionAsync(async () => await _service.AddAsync(deletedEntityId, entityType, deletedAt));
        Assert.NotNull(exception);
    }

    [Fact]
    public async void given_entity_id_is_in_database_exists_returns_true()
    {
        var deletedEntityId = Guid.NewGuid();
        await _dbContext.DeletedEntities.AddAsync(new DeletedEntityDto() { Id = deletedEntityId, EntityType = "foo", DeletedAt = DateTime.UtcNow });
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();

        var exists = await _service.ExistsAsync(deletedEntityId);
        Assert.True(exists);
    }

    [Fact]
    public async void given_entity_id_is_not_in_database_exists_returns_false()
    {
        var deletedEntityId = Guid.NewGuid();
        await _dbContext.DeletedEntities.AddAsync(new DeletedEntityDto() { Id = deletedEntityId, EntityType = "foo", DeletedAt = DateTime.UtcNow });
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();

        var exists = await _service.ExistsAsync(Guid.NewGuid());
        Assert.False(exists);
    }

    private readonly TestDatabase _testDb;
    private readonly DatingAppDbContext _dbContext;
    private readonly DeletedEntityService _service;
    public DeletedEntityServiceTests()
    {
        _testDb = new TestDatabase();
        _dbContext = _testDb.DbContext;
        _service = new DeletedEntityService(_dbContext);
    }

    public void Dispose()
    {
        _testDb.Dispose();
    }
}
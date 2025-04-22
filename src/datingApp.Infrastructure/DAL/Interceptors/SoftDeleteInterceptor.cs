using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using datingApp.Core.Common;

namespace datingApp.Infrastructure.DAL.Interceptors;

public class SoftDeleteInterceptor : SaveChangesInterceptor
{
    // https://blog.jetbrains.com/dotnet/2023/06/14/how-to-implement-a-soft-delete-strategy-with-entity-framework-core/
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is null) return result;

        foreach (var entry in eventData.Context.ChangeTracker.Entries())
        {
            if (entry is not { State: EntityState.Deleted, Entity: SoftDelete delete }) continue;
            entry.State = EntityState.Modified;
            delete.Delete();
        }

        return result;
    }
}
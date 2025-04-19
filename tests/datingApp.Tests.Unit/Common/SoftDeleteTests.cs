using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Common;
using Xunit;

namespace datingApp.Tests.Unit.Common;

public class SoftDeleteTests
{
    [Fact]
    public void soft_delete_test()
    {
        var softDelete = new SoftDelete();
        Assert.False(softDelete.IsDeleted);
        Assert.Null(softDelete.DeletedAt);

    }

    [Fact]
    public void after_delete_softdelete_IsDeleted_is_true_and_DeletedAt_is_set_to_utc_now()
    {
        var softDelete = new SoftDelete();

        softDelete.Delete();
        Assert.True(softDelete.IsDeleted);
        Assert.NotNull(softDelete.DeletedAt);
        Assert.True(softDelete.DeletedAt.Value < DateTimeOffset.UtcNow);
    }
}
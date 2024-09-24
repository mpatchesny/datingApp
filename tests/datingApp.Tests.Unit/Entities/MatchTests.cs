using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using Xunit;

namespace datingApp.Tests.Unit.Entities;

public class MatchTests
{
    [Fact]
    public void set_is_displayed_changes_enitity_state_1()
    {
        var match = new Match(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), false, false, null, DateTime.UtcNow);
        match.SetDisplayed(match.UserId1);
        Assert.True(match.IsDisplayedByUser1);
        Assert.False(match.IsDisplayedByUser2);
    }

    [Fact]
    public void set_is_displayed_changes_enitity_state_2()
    {
        var match = new Match(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), false, false, null, DateTime.UtcNow);
        match.SetDisplayed(match.UserId2);
        Assert.True(match.IsDisplayedByUser2);
        Assert.False(match.IsDisplayedByUser1);
    }

    [Fact]
    public void set_is_displayed_by_wrong_user_id_does_not_change_entity_state()
    {
        var match = new Match(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), false, false, null, DateTime.UtcNow);
        match.SetDisplayed(Guid.NewGuid());
        Assert.False(match.IsDisplayedByUser2);
        Assert.False(match.IsDisplayedByUser1);
    }
}
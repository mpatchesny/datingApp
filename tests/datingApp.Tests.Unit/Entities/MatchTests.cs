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
        var match = new Match(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"), false, false, null, DateTime.UtcNow);
        match.SetDisplayed(Guid.Parse("00000000-0000-0000-0000-000000000001"));
        Assert.Equal(match.IsDisplayedByUser1, true);
        Assert.Equal(match.IsDisplayedByUser2, false);
    }

    [Fact]
    public void set_is_displayed_changes_enitity_state_2()
    {
        var match = new Match(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"), false, false, null, DateTime.UtcNow);
        match.SetDisplayed(Guid.Parse("00000000-0000-0000-0000-000000000002"));
        Assert.Equal(match.IsDisplayedByUser2, true);
        Assert.Equal(match.IsDisplayedByUser1, false);
    }

    [Fact]
    public void set_is_displayed_by_wrong_user_id_does_not_change_entity_state()
    {
        var match = new Match(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"), false, false, null, DateTime.UtcNow);
        match.SetDisplayed(Guid.Parse("00000000-0000-0000-0000-000000000003"));
        Assert.Equal(match.IsDisplayedByUser2, false);
        Assert.Equal(match.IsDisplayedByUser1, false);
    }
}
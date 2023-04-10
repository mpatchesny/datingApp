using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.PhotoManagement;
using datingApp.Core.Entities;
using Xunit;

namespace datingApp.Tests.Unit.PhotoManagement;

public class PhotoOrdererTests
{
    [Fact]
    public void change_photo_order_with_oridinal_greater_than_current_should_return_list_with_proper_order()
    {
        var photos = new List<Photo>{
            new Photo(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "abc", 0),
            new Photo(Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "abc", 1),
            new Photo(Guid.Parse("00000000-0000-0000-0000-000000000003"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "abc", 2)
        };

        var orderer = new PhotoOrderer();
        var list = orderer.OrderPhotos(photos, Guid.Parse("00000000-0000-0000-0000-000000000001"), 2);
        Assert.Equal(list[0].Id, Guid.Parse("00000000-0000-0000-0000-000000000002"));
        Assert.Equal(list[1].Id, Guid.Parse("00000000-0000-0000-0000-000000000003"));
        Assert.Equal(list[2].Id, Guid.Parse("00000000-0000-0000-0000-000000000001"));
    }

    [Fact]
    public void change_photo_order_with_oridinal_lower_than_current_should_return_list_with_proper_order()
    {
        var photos = new List<Photo>{
            new Photo(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "abc", 0),
            new Photo(Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "abc", 1),
            new Photo(Guid.Parse("00000000-0000-0000-0000-000000000003"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "abc", 2)
        };

        var orderer = new PhotoOrderer();
        var list = orderer.OrderPhotos(photos, Guid.Parse("00000000-0000-0000-0000-000000000003"), 0);
        Assert.Equal(list[0].Id, Guid.Parse("00000000-0000-0000-0000-000000000003"));
        Assert.Equal(list[1].Id, Guid.Parse("00000000-0000-0000-0000-000000000001"));
        Assert.Equal(list[2].Id, Guid.Parse("00000000-0000-0000-0000-000000000002"));
    }

    [Fact]
    public void change_photo_order_with_oridinal_greater_than_list_count_should_place_photo_at_the_end()
    {
        var photos = new List<Photo>{
            new Photo(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "abc", 0),
            new Photo(Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "abc", 1),
            new Photo(Guid.Parse("00000000-0000-0000-0000-000000000003"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "abc", 2)
        };

        var orderer = new PhotoOrderer();
        var list = orderer.OrderPhotos(photos, Guid.Parse("00000000-0000-0000-0000-000000000001"), 4);
        Assert.Equal(list[0].Id, Guid.Parse("00000000-0000-0000-0000-000000000002"));
        Assert.Equal(list[1].Id, Guid.Parse("00000000-0000-0000-0000-000000000003"));
        Assert.Equal(list[2].Id, Guid.Parse("00000000-0000-0000-0000-000000000001"));
    }

    [Fact]
    public void change_photo_order_with_negative_oridinal_should_place_photo_at_the_beginning()
    {
        var photos = new List<Photo>{
            new Photo(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "abc", 0),
            new Photo(Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "abc", 1),
            new Photo(Guid.Parse("00000000-0000-0000-0000-000000000003"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "abc", 2)
        };

        var orderer = new PhotoOrderer();
        var list = orderer.OrderPhotos(photos, Guid.Parse("00000000-0000-0000-0000-000000000003"), -1);
        Assert.Equal(list[0].Id, Guid.Parse("00000000-0000-0000-0000-000000000003"));
        Assert.Equal(list[1].Id, Guid.Parse("00000000-0000-0000-0000-000000000001"));
        Assert.Equal(list[2].Id, Guid.Parse("00000000-0000-0000-0000-000000000002"));
    }

    [Fact]
    public void list_size_should_not_change_after_performing_order()
    {
        var photos = new List<Photo>{
            new Photo(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "abc", 0),
            new Photo(Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "abc", 1),
            new Photo(Guid.Parse("00000000-0000-0000-0000-000000000003"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "abc", 2)
        };

        var orderer = new PhotoOrderer();
        var list = orderer.OrderPhotos(photos, Guid.Parse("00000000-0000-0000-0000-000000000001"), 2);
        Assert.Equal(3, list.Count());
    }
}
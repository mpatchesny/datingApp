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
            new Photo(1, 1, "abc", 0),
            new Photo(2, 1, "abc", 1),
            new Photo(3, 1, "abc", 2)
        };

        var orderer = new PhotoOrderer();
        var list = orderer.OrderPhotos(photos, 1, 2);
        Assert.Equal(list[0].Id, 2);
        Assert.Equal(list[1].Id, 3);
        Assert.Equal(list[2].Id, 1);
    }

    [Fact]
    public void change_photo_order_with_oridinal_lower_than_current_should_return_list_with_proper_order()
    {
        var photos = new List<Photo>{
            new Photo(1, 1, "abc", 0),
            new Photo(2, 1, "abc", 1),
            new Photo(3, 1, "abc", 2)
        };

        var orderer = new PhotoOrderer();
        var list = orderer.OrderPhotos(photos, 3, 0);
        Assert.Equal(list[0].Id, 3);
        Assert.Equal(list[1].Id, 1);
        Assert.Equal(list[2].Id, 2);
    }

    [Fact]
    public void change_photo_order_with_oridinal_greater_than_list_count_should_place_photo_at_the_end()
    {
        var photos = new List<Photo>{
            new Photo(1, 1, "abc", 0),
            new Photo(2, 1, "abc", 1),
            new Photo(3, 1, "abc", 2)
        };

        var orderer = new PhotoOrderer();
        var list = orderer.OrderPhotos(photos, 1, 4);
        Assert.Equal(list[0].Id, 2);
        Assert.Equal(list[1].Id, 3);
        Assert.Equal(list[2].Id, 1);
    }

    [Fact]
    public void change_photo_order_with_negative_oridinal_should_place_photo_at_the_beginning()
    {
        var photos = new List<Photo>{
            new Photo(1, 1, "abc", 0),
            new Photo(2, 1, "abc", 1),
            new Photo(3, 1, "abc", 2)
        };

        var orderer = new PhotoOrderer();
        var list = orderer.OrderPhotos(photos, 3, -1);
        Assert.Equal(list[0].Id, 3);
        Assert.Equal(list[1].Id, 1);
        Assert.Equal(list[2].Id, 2);
    }

    [Fact]
    public void list_size_should_not_change_after_performing_order()
    {
        var photos = new List<Photo>{
            new Photo(1, 1, "abc", 0),
            new Photo(2, 1, "abc", 1),
            new Photo(3, 1, "abc", 2)
        };

        var orderer = new PhotoOrderer();
        var list = orderer.OrderPhotos(photos, 1, 2);
        Assert.Equal(3, list.Count());
    }
}
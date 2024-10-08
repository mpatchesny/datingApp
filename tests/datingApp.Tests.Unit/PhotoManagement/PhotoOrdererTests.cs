using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.PhotoManagement;
using datingApp.Core.Entities;
using datingApp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Xunit;

namespace datingApp.Tests.Unit.PhotoManagement;

public class PhotoOrdererTests
{
    [Fact]
    public void change_photo_order_with_oridinal_greater_than_current_should_return_list_with_proper_order()
    {
        var photos = new List<Photo>{
            CreatePhoto(0),
            CreatePhoto(1),
            CreatePhoto(2),
        };

        var orderer = new PhotoOrderer();
        var list = orderer.OrderPhotos(photos, photos[0].Id, 2);
        Assert.Equal(list[0].Id.ToString(), photos[1].Id.ToString());
        Assert.Equal(list[1].Id.ToString(), photos[2].Id.ToString());
        Assert.Equal(list[2].Id.ToString(), photos[0].Id.ToString());
    }

    [Fact]
    public void change_photo_order_with_oridinal_lower_than_current_should_return_list_with_proper_order()
    {
        var photos = new List<Photo>{
            CreatePhoto(0),
            CreatePhoto(1),
            CreatePhoto(2),
        };

        var orderer = new PhotoOrderer();
        var list = orderer.OrderPhotos(photos, photos[2].Id, 0);
        Assert.Equal(list[0].Id, photos[2].Id);
        Assert.Equal(list[1].Id, photos[0].Id);
        Assert.Equal(list[2].Id, photos[1].Id);
    }

    [Fact]
    public void change_photo_order_with_oridinal_greater_than_list_count_should_place_photo_at_the_end()
    {
        var photos = new List<Photo>{
            CreatePhoto(0),
            CreatePhoto(1),
            CreatePhoto(2),
        };

        var orderer = new PhotoOrderer();
        var list = orderer.OrderPhotos(photos, photos[0].Id, 4);
        Assert.Equal(list[0].Id, photos[1].Id);
        Assert.Equal(list[1].Id, photos[2].Id);
        Assert.Equal(list[2].Id, photos[0].Id);
    }

    [Fact]
    public void change_photo_order_with_negative_oridinal_should_place_photo_at_the_beginning()
    {
        var photos = new List<Photo>{
            CreatePhoto(0),
            CreatePhoto(1),
            CreatePhoto(2),
        };

        var orderer = new PhotoOrderer();
        var list = orderer.OrderPhotos(photos, photos[2].Id, -1);
        Assert.Equal(list[0].Id, photos[2].Id);
        Assert.Equal(list[1].Id, photos[0].Id);
        Assert.Equal(list[2].Id, photos[1].Id);
    }

    [Fact]
    public void list_size_should_not_change_after_performing_order()
    {
        var photos = new List<Photo>{
            CreatePhoto(0),
            CreatePhoto(1),
            CreatePhoto(2),
        };

        var orderer = new PhotoOrderer();
        var list = orderer.OrderPhotos(photos, photos[0].Id, 2);
        Assert.Equal(3, list.Count);
    }

    private Photo CreatePhoto(int oridinal)
    {
        return new Photo(photoFile.PhotoId, Guid.NewGuid(), "abc", oridinal);
    }
}
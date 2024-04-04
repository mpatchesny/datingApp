using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.DTO;
using datingApp.Core.Entities;
using Newtonsoft.Json;
using Xunit;

namespace datingApp.Tests.Integration.Controllers;

[Collection("Integration tests")]
public class PhotosControllerTests : ControllerTestBase, IDisposable
{
    [Fact]
    public async Task get_photo_returns_200_ok_and_photo_dto()
    {
        var user = await CreateUserAsync("test@test.com");
        var photo = await CreatePhotoAsync(user);

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");

        var response = await Client.GetFromJsonAsync<PhotoDto>($"photos/{photo.Id}");
        Assert.NotNull(response);
        Assert.Equal(photo.Id, response.Id);
    }

    [Fact]
    public async Task given_photo_not_exists_get_photo_returns_404_not_found()
    {
        var user = await CreateUserAsync("test@test.com");
        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");
        var response = await Client.GetAsync($"photos/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task given_valid_payload_post_photo_returns_201_created_and_photo_dto()
    {
        var user = await CreateUserAsync("test@test.com");

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");

        var photoBase64 = "/9j/4AAQSkZJRgABAQEAYABgAAD/4QAiRXhpZgAATU0AKgAAAAgAAQESAAMAAAABAAEAAAAAAAD/2wBDAAIBAQIBAQICAgICAgICAwUDAwMDAwYEBAMFBwYHBwcGBwcICQsJCAgKCAcHCg0KCgsMDAwMBwkODw0MDgsMDAz/2wBDAQICAgMDAwYDAwYMCAcIDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAz/wAARCAAWABcDASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwD9/KKKKACiiigAooooAKKKKAP/2Q==";
        var command = new AddPhoto(Guid.Empty, user.Id, photoBase64);
        var response = await Client.PostAsJsonAsync("/photos", command);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<PhotoDto>();
        Assert.Equal(user.Id, dto.UserId);
    }

    [Fact]
    public async Task given_empty_base64_photo_post_photo_returns_400_bad_request()
    {
        var user = await CreateUserAsync("test@test.com");

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");

        var command = new AddPhoto(Guid.Empty, user.Id, "");
        var response = await Client.PostAsJsonAsync("/photos", command);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task given_valid_payload_patch_photo_post_photo_returns_204_no_content()
    {
        var user = await CreateUserAsync("test@test.com");
        var photo = await CreatePhotoAsync(user);

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");

        var command = new ChangePhotoOridinal(photo.Id, 1);
        var payload = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");
        var response = await Client.PatchAsync($"/photos/{photo.Id}", payload);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task given_photo_not_exists_patch_photo_post_photo_returns_404_not_found()
    {
        var user = await CreateUserAsync("test@test.com");

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");

        var command = new ChangePhotoOridinal(Guid.NewGuid(), 0);
        var payload = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");
        var response = await Client.PatchAsync($"/photos/{Guid.NewGuid()}", payload);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task given_photo_exists_delete_photo_post_photo_returns_204_no_content()
    {
        var user = await CreateUserAsync("test@test.com");
        var photo = await CreatePhotoAsync(user);

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");

        var response = await Client.DeleteAsync($"/photos/{photo.Id}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task given_photo_not_exists_delete_photo_post_photo_returns_404_not_found()
    {
        var user = await CreateUserAsync("test@test.com");

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");

        var response = await Client.DeleteAsync($"/photos/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact (Skip = "FIXME")]
    public async Task given_photo_was_alread_deleted_delete_photo_post_photo_returns_410_gone()
    {
        var user = await CreateUserAsync("test@test.com");

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");

        var response = await Client.DeleteAsync($"/photos/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.Gone, response.StatusCode);
    }

    private async Task<User> CreateUserAsync(string email, string phone = null)
    {
        var userId = Guid.NewGuid();
        var settings = new UserSettings(userId, Sex.MaleAndFemale, 18, 100, 100, 45.5, 45.5);
        Random random = new Random();
        if (phone == null) phone = random.Next(100000000, 999999999).ToString();
        var user = new User(userId, phone, email, "Janusz", new DateOnly(2000,1,1), Sex.Male, null, settings);
        await _testDb.DbContext.Users.AddAsync(user);
        await _testDb.DbContext.SaveChangesAsync();
        return user;
    }

    private async Task<Photo> CreatePhotoAsync(User user)
    {
        var photo = new Photo(Guid.Empty, user.Id, "path", "url", 0);
        await _testDb.DbContext.Photos.AddAsync(photo);
        await _testDb.DbContext.SaveChangesAsync();
        return photo;
    }

    private readonly TestDatabase _testDb;
    public PhotosControllerTests(OptionsProvider optionsProvider) : base(optionsProvider)
    {
        _testDb = new TestDatabase();
    }
    public void Dispose()
    {
        _testDb?.Dispose();
    }
}
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
using datingApp.Application.PhotoManagement;
using datingApp.Core.Entities;
using Newtonsoft.Json;
using Xunit;

namespace datingApp.Tests.Integration.Controllers;

[Collection("Controller tests")]
public class PhotosControllerTests : ControllerTestBase, IDisposable
{
    [Fact]
    public async Task get_photo_returns_200_ok_and_photo_dto()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var photo = await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id);

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.GetFromJsonAsync<PhotoDto>($"photos/{photo.Id}");
        Assert.NotNull(response);
        Assert.Equal(photo.Id, response.Id);
    }

    [Fact]
    public async Task given_photo_not_exists_get_photo_returns_404_not_found_and_proper_error_reason()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var notExistingPhotoId = Guid.NewGuid();
        var response = await Client.GetAsync($"photos/{notExistingPhotoId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<Error>();
        Assert.Equal($"Photo with id {notExistingPhotoId} does not exist.", error.Reason);
    }

    [Fact]
    public async Task given_valid_payload_post_photo_returns_201_created_and_photo_dto()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

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
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var command = new AddPhoto(Guid.Empty, user.Id, "");
        var response = await Client.PostAsJsonAsync("/photos", command);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task given_valid_payload_patch_photo_post_photo_returns_204_no_content()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var photo = await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id);

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var command = new ChangePhotoOridinal(photo.Id, 1);
        var payload = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");
        var response = await Client.PatchAsync($"/photos/{photo.Id}", payload);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task given_photo_not_exists_patch_photo_returns_404_not_found_and_proper_error_reason()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var notExistingPhotoId = Guid.NewGuid();
        var command = new ChangePhotoOridinal(notExistingPhotoId, 0);
        var payload = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");
        var response = await Client.PatchAsync($"/photos/{notExistingPhotoId}", payload);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<Error>();
        Assert.Equal($"Photo with id {notExistingPhotoId} does not exist.", error.Reason);
    }

    [Fact]
    public async Task given_photo_exists_delete_photo_returns_204_no_content()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var photo = await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id);

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.DeleteAsync($"/photos/{photo.Id}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task given_photo_not_exists_delete_photo_returns_404_not_found_and_proper_error_reason()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var notExistingPhotoId = Guid.NewGuid();
        var response = await Client.DeleteAsync($"/photos/{notExistingPhotoId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<Error>();
        Assert.Equal($"Photo with id {notExistingPhotoId} does not exist.", error.Reason);
    }

    [Fact]
    public async Task given_photo_was_alread_deleted_delete_photo_returns_410_gone()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var photo = await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id);
        await IntegrationTestHelper.DeletePhotoAsync(_testDb, photo);

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.DeleteAsync($"/photos/{photo.Id}");
        Assert.Equal(HttpStatusCode.Gone, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<Error>();
        Assert.Equal($"Photo {photo.Id} is deleted permanently.", error.Reason);
    }

    [Fact]
    public async Task get_storage_returns_200_OK_and_photo_binary()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);
        var photo = await IntegrationTestHelper.CreatePhotoAsync(_testDb, user.Id);
        var photoBinary = new byte[]{
                0x1F, 0x8B, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0xFB, 0xFF,
                0x9F, 0xF6, 0x00, 0x00, 0x81, 0x86, 0xD2, 0x03, 0x64, 0x00, 0x00, 0x00
            };
        var photoBase64 = "/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////w==";
        var file = new FileDto() { Id = photo.Id, Extension = "jpg", Binary = photoBinary };
        await _testDb.DbContext.Files.AddAsync(file);
        await _testDb.DbContext.SaveChangesAsync();

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.GetAsync($"/storage/{photo.Id}.jpg");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(photoBase64, Convert.ToBase64String(await response.Content.ReadAsByteArrayAsync()));
    }

    [Fact]
    public async Task given_physical_file_not_exists_get_storage_returns_404_not_found()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_testDb);

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var notExistingPhotoFilename = Guid.NewGuid().ToString() + ".jpg";
        var response = await Client.GetAsync($"/storage/{notExistingPhotoFilename}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode); ;
    }

    private readonly TestDatabase _testDb;
    public PhotosControllerTests(OptionsProvider optionsProvider) : base(optionsProvider)
    {
        _testDb = new TestDatabase(false);
    }

    public void Dispose()
    {
        var tempFolder = System.IO.Path.Combine(Path.GetTempPath(), "datingapptest");
        System.IO.Directory.Delete(tempFolder, true);
        _testDb?.Dispose();
    }
}
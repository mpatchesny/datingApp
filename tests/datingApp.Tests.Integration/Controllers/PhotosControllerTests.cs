using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using datingApp.Application.Commands;
using datingApp.Application.DTO;
using datingApp.Core.Entities;
using datingApp.Infrastructure;
using datingApp.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Xunit;

namespace datingApp.Tests.Integration.Controllers;

[Collection("Controller tests")]
public class PhotosControllerTests : ControllerTestBase, IDisposable
{
    [Fact]
    public async Task get_photo_returns_200_ok_and_photo_dto()
    {
        var photos = new List<Photo>() { IntegrationTestHelper.CreatePhoto() };
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext, photos: photos);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.GetFromJsonAsync<PhotoDto>($"/photos/{photos[0].Id.Value}");

        Assert.NotNull(response);
        Assert.True(photos[0].Id.Equals(response.Id));
    }

    [Fact]
    public async Task given_photo_not_exists_get_photo_returns_404_not_found_and_proper_error_reason()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var notExistingPhotoId = Guid.NewGuid();
        var response = await Client.GetAsync($"/photos/{notExistingPhotoId}");
        var error = await response.Content.ReadFromJsonAsync<Error>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal($"Photo with id {notExistingPhotoId} does not exist.", error.Reason);
    }

    [Fact]
    public async Task given_valid_payload_post_photo_returns_201_created_and_photo_dto()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var fileContent = new StreamContent(IntegrationTestHelper.SamplePhotoStream());
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

        var formData = new MultipartFormDataContent();
        formData.Add(fileContent, "fileContent", "file.jpg");

        var response = await Client.PostAsync("/users/me/photos", formData);
        var dto = await response.Content.ReadFromJsonAsync<PhotoDto>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(dto);
        Assert.IsType<PhotoDto>(dto);
        Assert.Equal(user.Id.Value, dto.UserId);
    }

    [Fact]
    public async Task given_photo_already_exists_post_photo_returns_400_bad_request()
    {
        var stream = IntegrationTestHelper.SamplePhotoStream();

        stream.Position = 0;
        var checksum = "";
        using (var md5 = System.Security.Cryptography.MD5.Create())
        {
            var hashBytes = await md5.ComputeHashAsync(stream);
            checksum = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }

        var photos = new List<Photo>() { IntegrationTestHelper.CreatePhoto(checksum: checksum) };
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext, photos: photos);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

        var formData = new MultipartFormDataContent();
        formData.Add(fileContent, "fileContent", "file.jpg");

        var response = await Client.PostAsync("/users/me/photos", formData);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task given_empty_formfile_photo_post_photo_returns_400_bad_request()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var fileContent = new StreamContent(new MemoryStream());
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

        var formData = new MultipartFormDataContent();
        formData.Add(fileContent, "fileContent", "file.jpg");

        var response = await Client.PostAsync("/users/me/photos", formData);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task given_valid_payload_patch_photo_patch_photo_returns_204_no_content()
    {
        var photos = new List<Photo>() { IntegrationTestHelper.CreatePhoto() };
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext, photos: photos);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var command = new ChangePhotoOridinal(photos[0].Id, 1);
        var payload = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");
        var response = await Client.PatchAsync($"/photos/{photos[0].Id.Value}", payload);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Empty(await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task given_photo_not_exists_patch_photo_returns_404_not_found_and_proper_error_reason()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var notExistingPhotoId = Guid.NewGuid();
        var command = new ChangePhotoOridinal(notExistingPhotoId, 0);
        var payload = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");
        var response = await Client.PatchAsync($"/photos/{notExistingPhotoId}", payload);
        var error = await response.Content.ReadFromJsonAsync<Error>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal($"Photo with id {notExistingPhotoId} does not exist.", error.Reason);
    }

    [Fact]
    public async Task given_user_not_owns_photo_patch_photo_returns_403_forbidden_with_proper_error_reason()
    {
        var photos = new List<Photo>() { IntegrationTestHelper.CreatePhoto() };
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext, photos: photos);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user2.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var command = new ChangePhotoOridinal(photos[0].Id, 0);
        var payload = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");
        var response = await Client.PatchAsync($"/photos/{photos[0].Id}", payload);
        var error = await response.Content.ReadFromJsonAsync<Error>();

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal($"You don't have permission to perform this action.", error.Reason);
    }

    [Fact]
    public async Task given_photo_exists_delete_photo_returns_204_no_content()
    {
        var photos = new List<Photo>() { IntegrationTestHelper.CreatePhoto() };
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext, photos: photos);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.DeleteAsync($"/photos/{photos[0].Id.Value}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Empty(await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task given_photo_not_exists_delete_photo_returns_404_not_found_and_proper_error_reason()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var notExistingPhotoId = Guid.NewGuid();
        var response = await Client.DeleteAsync($"/photos/{notExistingPhotoId}");
        var error = await response.Content.ReadFromJsonAsync<Error>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal($"Photo with id {notExistingPhotoId} does not exist.", error.Reason);
    }

    [Fact]
    public async Task given_photo_was_already_deleted_delete_photo_returns_410_gone()
    {
        var photo = IntegrationTestHelper.CreatePhoto();
        var photos = new List<Photo>() { photo };
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext, photos: photos);
        await IntegrationTestHelper.DeletePhotoAsync(_dbContext, photos[0]);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.DeleteAsync($"/photos/{photo.Id.Value}");
        var error = await response.Content.ReadFromJsonAsync<Error>();

        Assert.Equal(HttpStatusCode.Gone, response.StatusCode);
        Assert.Equal($"Photo {photo.Id.Value} is deleted permanently.", error.Reason);
    }

    [Fact]
    public async Task given_user_not_owns_photo_delete_photo_returns_403_forbidden_with_proper_error_reason()
    {
        var photos = new List<Photo>() { IntegrationTestHelper.CreatePhoto() };
        var user1 = await IntegrationTestHelper.CreateUserAsync(_dbContext, photos: photos);
        var user2 = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user2.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.DeleteAsync($"/photos/{photos[0].Id.Value}");
        var error = await response.Content.ReadFromJsonAsync<Error>();

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal($"You don't have permission to perform this action.", error.Reason);
    }

    private readonly TestDatabase _testDb;
    private readonly DatingAppDbContext _dbContext;

    public PhotosControllerTests(OptionsProvider optionsProvider) : base(optionsProvider)
    {
        _testDb = new TestDatabase(false);
        _dbContext = _testDb.DbContext;
    }

    public void Dispose()
    {
        _testDb?.Dispose();
    }
}
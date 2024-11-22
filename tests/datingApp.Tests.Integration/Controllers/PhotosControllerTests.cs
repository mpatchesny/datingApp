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
using datingApp.Infrastructure;
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

        var response = await Client.GetFromJsonAsync<PhotoDto>($"photos/{photos[0].Id.Value}");
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
        var response = await Client.GetAsync($"photos/{notExistingPhotoId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<Error>();
        Assert.Equal($"Photo with id {notExistingPhotoId} does not exist.", error.Reason);
    }

    [Fact]
    public async Task given_valid_payload_post_photo_returns_201_created_and_photo_dto()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var command = new AddPhoto(Guid.Empty, user.Id, IntegrationTestHelper.SamplePhotoStream());
        var response = await Client.PostAsJsonAsync("/photos", command);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var dto = await response.Content.ReadFromJsonAsync<PhotoDto>();
        Assert.Equal(user.Id.Value, dto.Id);
    }

    [Fact]
    public async Task given_empty_base64_photo_post_photo_returns_400_bad_request()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var command = new AddPhoto(Guid.Empty, user.Id, IntegrationTestHelper.SamplePhotoStream());
        var response = await Client.PostAsJsonAsync("/photos", command);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task given_valid_payload_patch_photo_post_photo_returns_204_no_content()
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
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<Error>();
        Assert.Equal($"Photo with id {notExistingPhotoId} does not exist.", error.Reason);
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
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<Error>();
        Assert.Equal($"Photo with id {notExistingPhotoId} does not exist.", error.Reason);
    }

    [Fact]
    public async Task given_photo_was_alread_deleted_delete_photo_returns_410_gone()
    {
        var photo = IntegrationTestHelper.CreatePhoto();
        var photos = new List<Photo>() { photo };
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext, photos: photos);
        await IntegrationTestHelper.DeletePhotoAsync(_dbContext, photos[0]);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var response = await Client.DeleteAsync($"/photos/{photo.Id.Value}");
        Assert.Equal(HttpStatusCode.Gone, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<Error>();
        Assert.Equal($"Photo {photo.Id.Value} is deleted permanently.", error.Reason);
    }

    [Fact]
    public async Task get_storage_returns_200_OK_and_photo_binary()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var command = new AddPhoto(Guid.Empty, user.Id, IntegrationTestHelper.SamplePhotoStream());
        var postResponse = await Client.PostAsJsonAsync("/photos", command);
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
        var photoDto = await postResponse.Content.ReadFromJsonAsync<PhotoDto>();
        var photoId = photoDto.Id;

        var response = await Client.GetAsync($"/storage/{photoId}.jpg");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task given_physical_file_not_exists_get_storage_returns_404_not_found()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var notExistingPhotoFilename = Guid.NewGuid().ToString() + ".jpg";
        var response = await Client.GetAsync($"/storage/{notExistingPhotoFilename}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode); ;
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
        var tempFolder = System.IO.Path.Combine(Path.GetTempPath(), "datingapptest");
        System.IO.Directory.Delete(tempFolder, true);
        _testDb?.Dispose();
    }
}
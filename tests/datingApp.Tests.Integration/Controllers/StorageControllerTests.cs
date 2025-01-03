using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Infrastructure;
using Xunit;

namespace datingApp.Tests.Integration.Controllers;

[Collection("Controller tests")]
public class StorageControllerTests : ControllerTestBase, IDisposable
{
    [Fact]
    public async Task get_storage_returns_200_OK_and_photo_binary()
    {
        var user = await IntegrationTestHelper.CreateUserAsync(_dbContext);
        _dbContext.ChangeTracker.Clear();

        var token = Authorize(user.Id);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken.Token}");

        var photoStreamLength = IntegrationTestHelper.SamplePhotoStream().Length;
        var fileContent = new StreamContent(IntegrationTestHelper.SamplePhotoStream());
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");

        var formData = new MultipartFormDataContent();
        formData.Add(fileContent, "fileContent", "file.png");

        var postResponse = await Client.PostAsync("/users/me/photos", formData);
        var photoDto = await postResponse.Content.ReadFromJsonAsync<PhotoDto>();

        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
        // remove ~ from the beginning, otherwise it won't work
        var response = await Client.GetAsync(photoDto.Url.Trim('~'));
        var content = await response.Content.ReadAsByteArrayAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.InRange(content.Length, 1, photoStreamLength);
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

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private readonly TestDatabase _testDb;
    private readonly DatingAppDbContext _dbContext;

    public StorageControllerTests(OptionsProvider optionsProvider) : base(optionsProvider)
    {
        _testDb = new TestDatabase(false);
        _dbContext = _testDb.DbContext;
    }

    public void Dispose()
    {
        var tempFolder = System.IO.Path.Combine(Path.GetTempPath(), "datingapptest");
        System.IO.Directory.Delete(tempFolder, true);
        _testDb.Dispose();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace datingApp.Api.Controllers;

[Route("storage")]
public class StaticController : ControllerBase
{
    private readonly IOptions<StorageOptions> _storageOptions;
    public StaticController(IOptions<StorageOptions> storageOptions)
    {
        _storageOptions = storageOptions;
    }

    [Authorize]
    [HttpGet("{filename}")]
    public ActionResult Get(string filename)
    {
        var filePath = Path.Combine(_storageOptions.Value.StoragePath, filename);
        if (System.IO.File.Exists(filePath))
        {
            return PhysicalFile(filePath, "image/jpeg");
        }
        else
        {
            return NotFound();
        }
    }
}
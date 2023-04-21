using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace datingApp.Api.Controllers;

[Route("storage")]
public class StaticController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    public StaticController(IWebHostEnvironment env)
    {
        _env = env;
    }

    [Authorize]
    [HttpGet("{filename}")]
    public ActionResult Get(string filename)
    {
        var filePath = Path.Combine(_env.ContentRootPath, "storage", filename);
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
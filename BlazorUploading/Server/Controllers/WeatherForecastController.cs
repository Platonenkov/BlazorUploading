using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using BlazorUploading.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BlazorUploading.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost]
        [Route("SendOne")]
        [DisableRequestSizeLimit]
        public async Task<ActionResult> SendOne([FromForm] IFormFile file)
        {
            try
            {
                if (file is null)
                {
                    file = Request.Form.Files.FirstOrDefault();
                    if (file is null)
                    {
                        Debug.WriteLine("No Data");
                        return null;
                    }
                    Debug.WriteLine(file.Name);

                }

                return Ok(file.FileName);
            }
            catch (Exception e)
            {
                return BadRequest($"File have errors: {e.Message}");
            }
        }
        [HttpPost]
        [Route("SendMany")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> SendMany([FromForm] IFormFile[] files)
        {
            if (files is null || files.Length == 0)
            {
                files = Request.Form.Files.ToArray();
                if (files.Length == 0)
                {
                    Debug.WriteLine("No Data");
                    return null;
                }

            }
            var results = new List<string>();
            var current = 1;
            foreach (var file in files.AsParallel())
            {
                Debug.WriteLine($"Чтение файла {current} из {files.Length}");
                current++;
                var result = file.FileName;
                results.Add(result);
            }

            return Ok(results);

        }
        [HttpPost]
        [Route("Upload")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (file is null || file.Length == 0)
            {
                return BadRequest("Upload a file");
            }

            var responce = $"Загружено {file.FileName}";
            return Ok($"{responce}");
        }

    }
}

using EFBlazorBasics_Wasm.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFBlazorBasics_Wasm.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SampleActivitysController : ControllerBase
    {
        string ActivitysJson = "[{\"Round\":{\"No\":1},\"Helper\":{\"Name\":\"John Marshall\"}, \"Task\":\"Shot Put\"},{ \"Round\":{ \"No\":2},\"Helper\":{ \"Name\":\"Sue Burrows\"},\"Task\":\"Marshalling\"},{ \"Round\":{ \"No\":3},\"Helper\":{ \"Name\":\"Jimmy Beans\"},\"Task\":\"Discus\"}]";

        private readonly ILogger<SampleActivitysController> _logger;

        public SampleActivitysController(ILogger<SampleActivitysController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Activity> Get()
        {
            var activitys = JsonConvert.DeserializeObject<List<Activity>>(ActivitysJson).ToArray();
            return activitys;
        }

    }
}

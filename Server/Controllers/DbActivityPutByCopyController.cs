using EFBlazorBasics_Wasm.Server.Data;
using EFBlazorBasics_Wasm.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class DbActivityPutByCopyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHelperService _service;
        public DbActivityPutByCopyController(ApplicationDbContext context)
        {
            this._context = context;
            this._service = new HelperService(context);
        }


        [HttpPut]
        public async Task<IActionResult> Put(Activity activity)
        {
            await _service.UpdateActivityByCopy(activity);
            return Ok();
        }

    }
}

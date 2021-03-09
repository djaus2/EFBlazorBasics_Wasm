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
    public class DbHelpersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHelperService _service;
        public DbHelpersController(ApplicationDbContext context)
        {
            this._context = context;
            this._service = new HelperService(context);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var helpers = await  _service.GetHelpers();
            return Ok(helpers);
        }

        [HttpGet("{Ids}")]
        public async Task<IActionResult> Get(string Ids)
        {
            int Id;
            if (int.TryParse(Ids, out Id))
            {
                var helpers = from h in _context.Helpers where h.Id == Id select h;
                return Ok(await helpers.SingleAsync());
            }
            else
                return StatusCode(400);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Helper helper)
        {
            await _service.AddHelper(helper);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Put(Helper helper)
        {
            await _service.UpdateHelper(helper);
            return Ok();
        }

        [HttpDelete("{Ids}")]
        public async Task<IActionResult> Delete(string Ids)
        {
            int Id;
            if (int.TryParse(Ids, out Id))
            {
                await _service.DeleteHelper(Id);
                return Ok();
            }
            else
                return NotFound();
        }

    }
}

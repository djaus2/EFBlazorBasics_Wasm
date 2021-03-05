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
    public class DbRoundsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHelperService _service;
        public DbRoundsController(ApplicationDbContext context, IHelperService service)
        {
            this._context = context;
            this._service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var helpers = await _service.GetRounds();
            return Ok(helpers);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Round round)
        {
            await _service.AddRound(round);
            return Ok();
        }


        [HttpDelete("{Ids}")]
        public async Task<IActionResult> Delete(string Ids)
        {
            int Id = int.Parse(Ids);
            await _service.DeleteRound(Id);
            return Ok();
        }

    }
}

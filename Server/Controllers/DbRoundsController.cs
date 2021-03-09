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
        public DbRoundsController(ApplicationDbContext context)
        {
            this._context = context;
            this._service = new HelperService(context);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var helpers = await _service.GetRounds();
            return Ok(helpers);
        }

        [HttpGet("{Ids}")]
        public async Task<IActionResult> Get(string Ids)
        {
            int Id;
            if (int.TryParse(Ids, out Id))
            {
                var rounds = from r in _context.Rounds where r.Id == Id select r;
                if (rounds.Count() ==0)
                    return NotFound();
                return Ok(await rounds.SingleAsync());
            }
            else
                return StatusCode(400);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Round round)
        {
            await _service.AddRound(round);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Put(Round round)
        {
            await _service.UpdateRound(round);
            return Ok();
        }


        [HttpDelete("{Ids}")]
        public async Task<IActionResult> Delete(string Ids)
        {
            int Id;
            if (int.TryParse(Ids, out Id))
            {
                await _service.DeleteRound(Id);
                return Ok();
            }
            else
                return StatusCode(400);
        }

    }
}

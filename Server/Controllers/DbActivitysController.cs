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
    public class DbActivitysController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHelperService _service;
        public DbActivitysController(ApplicationDbContext context)
        {
            this._context = context;
            this._service = new HelperService(context);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var activtys = await  _service.GetActivitys();
            //// Ref: https://stackoverflow.com/questions/13510204/json-net-self-referencing-loop-detected :
            //string js = JsonConvert.SerializeObject(activtys, new JsonSerializerSettings() {
            //    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            //});
            return Ok(activtys);
        }

        [HttpGet("{Ids}")]
        public async Task<IActionResult> Get(string Ids)
        {
            int Id;
            if (int.TryParse(Ids, out Id))
            {
                var act = from a in _context.Activitys where a.Id == Id select a;
                return Ok(await act.SingleAsync());
            }
            else
                return StatusCode(400);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Activity activity)
        {
            await _service.AddActivity(activity);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Put(Activity activity)
        {
            var routes = Request.RouteValues; //.QueryString;
            var asaf = Request.QueryString;
            await _service.UpdateActivity(activity);
            return Ok();
        }

        [HttpDelete("{Ids}")]
        public async Task<IActionResult> Delete(string Ids)
        {
            int Id;
            if (int.TryParse(Ids, out Id))
            {
                await _service.DeleteActivity(Id);
                return Ok();
            }
            else
                return NotFound();
        }
       
    }
}

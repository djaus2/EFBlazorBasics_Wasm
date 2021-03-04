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
        public DbActivitysController(ApplicationDbContext context, IHelperService service)
        {
            this._context = context;
            this._service = service;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> LoadDb()
        {
            var ResultOK = new Microsoft.AspNetCore.Mvc.StatusCodeResult(200); //Ok
            var ResultNOK = new Microsoft.AspNetCore.Mvc.StatusCodeResult(500); //NOk
            await AddSomeData();
            return ResultOK;

        }

        //[HttpGet("[action]")]
        //public async Task<IActionResult> Activitys()
        //{
        //    var ResultOK = new Microsoft.AspNetCore.Mvc..StatusCodeResult(200); //Ok
        //    var ResultNOK = new Microsoft.AspNetCore.Mvc.StatusCodeResult(500); //NOk
        //    await AddSomeData();
        //    return ResultOK;

        //}

        //[HttpGet]
        //public async Task<Activity[]> Get()
        //{
        //    var list = await _context.Activitys.ToArrayAsync(); //.Include(activity => activity.Helper).Include(activity => activity.Round).ToArrayAsync();
        //    return list;
        //}

        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Activitys.ToListAsync());
        //}

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

        [HttpPost]
        public async Task<IActionResult> Post(Activity activity)
        {
            await _service.AddActivity(activity);
            return Ok();
        }

        //[HttpGet]
        //public async Task<List<Helper>> Get()
        //{
        //    var list = await _context.Helpers.ToListAsync<Helper>();
        //    return list;
        //}

        //[HttpGet]
        //public async Task<List<Round>> Get()
        //{
        //    var list = await _context.Rounds.ToListAsync<Round>();
        //    return list;
        //}

        /// <summary>
        /// Generate some avtivities, with generated rounds and helpers.
        /// </summary>
        string ActivitysJson = "[{\"Round\":{\"No\":1},\"Helper\":{\"Name\":\"John Marshall\"}, \"Task\":\"Shot Put\"},{ \"Round\":{ \"No\":2},\"Helper\":{ \"Name\":\"Sue Burrows\"},\"Task\":\"Marshalling\"},{ \"Round\":{ \"No\":3},\"Helper\":{ \"Name\":\"Jimmy Beans\"},\"Task\":\"Discus\"}]";
        public async Task AddSomeData()
        {
            var activitys = JsonConvert.DeserializeObject<List<Activity>>(ActivitysJson);
            await AddActivitys(activitys);
        }

        public async Task AddActivitys(List<Activity> activitys)
        {
            // Clear any records first
            if (_context.Rounds.Count() != 0)
                _context.Rounds.RemoveRange(_context.Rounds.ToList());
            if (_context.Activitys.Count() != 0)
                _context.Activitys.RemoveRange(_context.Activitys.ToList());
            if (_context.Helpers.Count() != 0)
                _context.Helpers.RemoveRange(_context.Helpers.ToList());
            await _context.SaveChangesAsync();
            // Reset seeds
            await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT('Rounds', RESEED, 0)");
            await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT('Helpers', RESEED, 0)");
            await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT('Activitys', RESEED, 0)");
            // Save all
            _context.Activitys.AddRange(activitys);

            //if (contextSaveChangesAsync)
                await _context.SaveChangesAsync();
        }
    }
}

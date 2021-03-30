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
    public class DbAppController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHelperService _service;
        public DbAppController(ApplicationDbContext context)
        {
            this._context = context;
            this._service = new HelperService(context);
        }


        [HttpGet("[action]")]
        public IActionResult GetContextSaveChanges()
        {
            bool res = _service.GetContextSaveChangesAsync();
            return Ok(res); 
        }

        [HttpGet("[action]")]
        public IActionResult ToggleContextSaveChanges()
        {
            bool res = _service.GetContextSaveChangesAsync();
            _service.SetContextSaveChangesAsync(!res);
            res = _service.GetContextSaveChangesAsync();
            return Ok(res);
        }

        [HttpGet("[action]")]
        public IActionResult GetMarkContextEntityChanged()
        {
            bool res = _service.GetMarkContextEntityStateAsChanged();
            return Ok(res);
        }

        [HttpGet("[action]")]
        public IActionResult ToggleMarkContextEntityChanged()
        {
            bool res = _service.GetMarkContextEntityStateAsChanged();
            _service.SetMarkContextEntityStateAsChanged(!res);
            res = _service.GetMarkContextEntityStateAsChanged();
            return Ok(res);
        }


        [HttpGet("[action]")]
        public IActionResult GetNullActivityHelpersBeforeDeletingHelper()
        {
            bool res = _service.GetNullActivityHelpersBeforeDeletingHelper();
            return Ok(res);
        }

        [HttpGet("[action]")]
        public IActionResult ToggleNullActivityHelpersBeforeDeletingHelper()
        {
            bool res = _service.GetNullActivityHelpersBeforeDeletingHelper();
            _service.SetNullActivityHelpersBeforeDeletingHelper(!res);
            res = _service.GetNullActivityHelpersBeforeDeletingHelper();
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> LoadDb()
        {
            var ResultOK = new Microsoft.AspNetCore.Mvc.StatusCodeResult(200); //Ok
            var ResultNOK = new Microsoft.AspNetCore.Mvc.StatusCodeResult(500); //NOk
            await AddSomeData();
            return ResultOK;

        }

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
            ///Restore these static variables
            _service. SetContextSaveChangesAsync(true);
            _service.SetMarkContextEntityStateAsChanged(false);
            _service.SetNullActivityHelpersBeforeDeletingHelper(true);
           
            bool wasChanged = false;
            // Clear any records first
            if (_context.Rounds.Count() != 0)
            {
                _context.Rounds.RemoveRange(_context.Rounds.ToList());
                wasChanged = true;
            }
            if (_context.Activitys.Count() != 0)
            {
                _context.Activitys.RemoveRange(_context.Activitys.ToList());
                wasChanged = true;
            }
            if (_context.Helpers.Count() != 0)
            {
                _context.Helpers.RemoveRange(_context.Helpers.ToList());
                wasChanged = true;
            }
            if (wasChanged)
            {
                await _context.SaveChangesAsync();
            }
          
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

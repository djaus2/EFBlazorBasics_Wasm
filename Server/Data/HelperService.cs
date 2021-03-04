using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using EFBlazorBasics_Wasm.Shared;

namespace EFBlazorBasics_Wasm.Server.Data
{
        public interface IHelperService
        {
            void SetContextSaveChangesAsync(bool save);
            bool GetContextSaveChangesAsync();
            void SetMarkContextEntityStateAsChanged(bool mark);
            bool GetMarkContextEntityStateAsChanged();
            Task<List<Activity>> GetActivitys();
            Task<List<Helper>> GetHelpers();
            Task<List<Round>> GetRounds();
            Task AddSomeData();
            Task UpdateActivityTask(int ActivityId, string newTask);
            Task UpdateActivityHelper(int ActivityId, Helper helper);
            Task UpdateActivity(Activity activity);
            Task UpdateActivityByCopy(Activity activity);
            Task AddActivitys(List<Activity> activitys);
            Task AddActivity(Activity activity);
            //Task AddRounds(List<Round> rounds);
            //Task AddHelpers(List<Helper> helpers);
            Task DeleteHelper(int Id);
            Task DeleteRound(int Id);
            Task DeleteActivity(int Id);
        }

        public class HelperService : IHelperService
        {
            private readonly ApplicationDbContext _context;
            public HelperService(ApplicationDbContext context)
            {
                _context = context;
            }


            private bool contextSaveChangesAsync { get; set; } = true;
            public void SetContextSaveChangesAsync(bool save)
            {
                contextSaveChangesAsync = save;
            }
            public bool GetContextSaveChangesAsync()
            {
                return contextSaveChangesAsync;
            }
            private bool markContextEntityStateAsChanged { get; set; } = false;
            public void SetMarkContextEntityStateAsChanged(bool mark)
            {
                markContextEntityStateAsChanged = mark;
            }
            public bool GetMarkContextEntityStateAsChanged()
            {
                return markContextEntityStateAsChanged;
            }

            public async Task<List<Activity>> GetActivitys()
            {
                var list = await _context.Activitys.Include(activity => activity.Helper).Include(activity => activity.Round).ToListAsync();
                return list;
            }

            public async Task<List<Helper>> GetHelpers()
            {
                var list = await _context.Helpers.ToListAsync<Helper>();
                return list;
            }

            public async Task<List<Round>> GetRounds()
            {
                var list = await _context.Rounds.ToListAsync<Round>();
                return list;
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

                if (contextSaveChangesAsync)
                    await _context.SaveChangesAsync();
            }

            /// <summary>
            /// Deleting a helper doesn't delete its activities
            /// </summary>
            public async Task DeleteHelper(int Id)
            {
                var helpers = from h in _context.Helpers where h.Id == Id select h;
                Helper helperdb = helpers.FirstOrDefault();
                if (helperdb != null)
                {
                    _context.Helpers.Remove(helperdb);
                    if (markContextEntityStateAsChanged)
                        _context.Entry(helperdb).State = EntityState.Modified;
                    if (contextSaveChangesAsync)
                        await _context.SaveChangesAsync();
                }
            }

            /// <summary>
            /// Deleting an activity doesn't require its round nor helper to be deleted.
            /// </summary>
            public async Task DeleteActivity(int Id)
            {
                var activitys = from a in _context.Activitys where a.Id == Id select a;
                Activity activitydb = activitys.FirstOrDefault();
                if (activitydb != null)
                {
                    _context.Activitys.Remove(activitydb);
                    if (markContextEntityStateAsChanged)
                        _context.Entry(activitydb).State = EntityState.Modified;
                    if (contextSaveChangesAsync)
                        await _context.SaveChangesAsync();
                }
            }

            /// <summary>
            /// Cascade Deletion
            /// If a round is deleted then all of its activities need deletion.
            /// </summary>
            public async Task DeleteRound(int Id)
            {
            var rounds = _context.Rounds.Where(e => e.Id == Id).Include(e => e.Activitys);
                Round rounddb = rounds.FirstOrDefault();
                if (rounddb != null)
                {
                    _context.Rounds.Remove(rounddb);
                    if (markContextEntityStateAsChanged)
                        _context.Entry(rounddb).State = EntityState.Modified;
                    if (contextSaveChangesAsync)
                        await _context.SaveChangesAsync();
                }
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

            public async Task AddActivity(Activity activity)
            {
                _context.Activitys.Add(activity);
                if (contextSaveChangesAsync)
                    await _context.SaveChangesAsync();
            }

            public async Task UpdateActivityTask(int ActivityId, string newTask)
            {
                var activitys = await GetActivitys();
                var active = from a in activitys where a.Id == ActivityId select a;
                Activity activitydb = active.FirstOrDefault();
                if (activitydb != null)
                {
                    activitydb.Task = newTask;
                    _context.Activitys.Update(activitydb);
                    if (markContextEntityStateAsChanged)
                        _context.Entry(activitydb).State = EntityState.Modified;
                    if (contextSaveChangesAsync)
                        await _context.SaveChangesAsync();
                }

            }

            public async Task UpdateActivityHelper(int ActivityId, Helper helper)
            {
                var activitys = await GetActivitys();
                var active = from a in activitys where a.Id == ActivityId select a;
                Activity activitydb = active.FirstOrDefault();
                if (activitydb != null)
                {
                    activitydb.Helper = helper;
                    _context.Activitys.Update(activitydb);
                    if (markContextEntityStateAsChanged)
                        _context.Entry(activitydb).State = EntityState.Modified;
                    if (contextSaveChangesAsync)
                        await _context.SaveChangesAsync();
                }

            }
            public async Task UpdateActivity(Activity activity)
            {
                if (activity != null)
                {
                    _context.Activitys.Update(activity);
                    if (markContextEntityStateAsChanged)
                        _context.Entry(activity).State = EntityState.Modified;
                    if (contextSaveChangesAsync)
                        await _context.SaveChangesAsync();
                }
            }

            public async Task UpdateActivityByCopy(Activity activity)
            {
                var activitys = await GetActivitys();
                var active = from a in activitys where a.Id == activity.Id select a;
                Activity activitydb = active.FirstOrDefault();
                if (activitydb != null)
                {
                    if (activity != null)
                    {
                        activitydb.Round = activity.Round;
                        activitydb.Helper = activity.Helper;
                        activitydb.Task = activity.Task;
                        _context.Activitys.Update(activitydb);
                        if (markContextEntityStateAsChanged)
                            _context.Entry(activitydb).State = EntityState.Modified;
                        if (contextSaveChangesAsync)
                            await _context.SaveChangesAsync();
                    }
                }
            }
        }
    }


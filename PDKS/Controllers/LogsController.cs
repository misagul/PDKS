using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PDKS.Data;
using PDKS.Models;

namespace PDKS.Controllers
{
    public class LogsController : Controller
    {

        public readonly TimeSpan[] times = {
            new TimeSpan(8, 0, 0),      // 0
            new TimeSpan(8, 30, 0),     // 1
            new TimeSpan(9, 0, 0),      // 2
            new TimeSpan(9, 30, 0),     // 3
            new TimeSpan(10, 0, 0),     // 4
            new TimeSpan(10, 30, 0),    // 5
            new TimeSpan(11, 0, 0),     // 6
            new TimeSpan(11, 30, 0),    // 7
            new TimeSpan(12, 0, 0),     // 8
            new TimeSpan(13, 30, 0),    // 9
            new TimeSpan(14, 0, 0),     // 10
            new TimeSpan(14, 30, 0),    // 11
            new TimeSpan(15, 0, 0),     // 12
            new TimeSpan(15, 30, 0),    // 13
            new TimeSpan(16, 0, 0),     // 14
        };

        private readonly PDKSDbContext _dbContext;
        public LogsController(PDKSDbContext _dbContext)
        {
            this._dbContext = _dbContext;
        }
        public async Task<IActionResult> Index()
        {
            var logs = await _dbContext.Logs.Include("User").OrderBy(x=>x.User.Username).ThenBy(x=>x.Shift).ToListAsync();

			return View(logs);
        }

        public int CheckStatus(User user, int curShift)
        {
            int userShift = user.Shift;
            TimeSpan curTimeSpan = DateTime.Now.TimeOfDay;
            TimeSpan dif = curTimeSpan - times[curShift];

            if (dif.Minutes < 0)
            {
                HttpContext.Session.SetString("pressStatus", "You came early.");
                return -1;

            }
            else if (dif.Minutes > 5)
            {
                HttpContext.Session.SetString("pressStatus", "You came late.");
                var log = new Log
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Shift = user.Shift,
                    OnTime = false,
                };
                _dbContext.Logs.Add(log);
                user.Shift += 1;
                _dbContext.SaveChanges();

                return 0;
            }
            else
            {
                HttpContext.Session.SetString("pressStatus", "You came on time.");

                var log = new Log
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Shift = user.Shift,
                    OnTime = true,
                };
                _dbContext.Logs.Add(log);
                user.Shift += 1;
                _dbContext.SaveChanges();
                
                return 1;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Press()
        {
            string username = HttpContext.Session.GetString("loggedUser");
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Username == username);
            int curShift = -1;

            TimeSpan curTimeSpan = DateTime.Now.TimeOfDay;
            if (curTimeSpan < new TimeSpan(12, 5, 0))
            {
                curShift = (int)Math.Round((curTimeSpan.TotalMinutes - new TimeSpan(8, 0, 0).TotalMinutes) / 30);
            }
            else if (curTimeSpan < new TimeSpan(13, 30, 0))
            {
                HttpContext.Session.SetString("pressStatus", "You came on lunch time.");
                curShift = -1;
            }
            else if (curTimeSpan < new TimeSpan(16, 5, 0))
            {
                curShift = (int)Math.Round((curTimeSpan.TotalMinutes - new TimeSpan(9, 0, 0).TotalMinutes) / 30);
            }

            if (curShift != -1)
            {
                if (curShift == user.Shift)
                {
                    CheckStatus(user, curShift);
                }
                else if (curShift > user.Shift)
                {
                    for(int i=user.Shift;i<curShift;i++)
                    {
                        var log = new Log
                        {
                            Id = Guid.NewGuid(),
                            UserId = user.Id,
                            Shift = i,
                            OnTime = false,
                        };
                        _dbContext.Logs.Add(log);
                    }
                    user.Shift = curShift;

                    _dbContext.SaveChanges();
                    
                    CheckStatus(user, curShift);

                }
                else
                {
                    HttpContext.Session.Remove("pressStatus");
                }
            }
            HttpContext.Session.SetString("curShift", curShift.ToString());

            return RedirectToAction("Index", "Home");
        }

    }
}

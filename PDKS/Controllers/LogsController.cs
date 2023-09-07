using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PDKS.Data;

namespace PDKS.Controllers
{
    public class LogsController : Controller
    {
        private readonly PDKSDbContext _dbContext;
        public LogsController(PDKSDbContext _dbContext)
        {
            this._dbContext = _dbContext;
        }
        public async Task<IActionResult> Index()
        {
            var logs = await _dbContext.Logs.Include("User").ToListAsync();

			return View(logs);
        }

    }
}

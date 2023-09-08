using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PDKS.Data;
using PDKS.Models;
using System.Collections;

namespace PDKS.Controllers
{
    public class UsersController : Controller
    {
        private readonly PDKSDbContext _dbContext;
        public UsersController(PDKSDbContext _dbContext)
        {
            this._dbContext = _dbContext;
        }

		public async Task<IActionResult> Index()
		{
            var users = await _dbContext.Users.ToListAsync();
			return View(users);
		}

		[HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddUserViewModel addUserRequest)
        {
            var user = await _dbContext.Users.AnyAsync(x => x.Username == addUserRequest.Username);
            if (!user)
            {
                
                var newUser = new User
                {
                    Id = Guid.NewGuid(),
                    Username = addUserRequest.Username,
                    Password = addUserRequest.Password,
                };

                await _dbContext.Users.AddAsync(newUser);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            else
            {
                TempData["addUserError"] = "Username already exist";
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user != null) {
                var viewModel = new UpdateUserViewModel
                {
                    Id = user.Id,
                    Username = user.Username,
                    Password = user.Password,
                    IsActive = user.IsActive,
                    Shift = user.Shift,
                };

                return View(viewModel);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateUserViewModel updateUserRequest)
        {
            var user = await _dbContext.Users.FindAsync(updateUserRequest.Id);

            if (user != null)
            {
                user.Username = updateUserRequest.Username;
                user.Password = updateUserRequest.Password;
                user.IsActive = updateUserRequest.IsActive;
                user.Shift = updateUserRequest.Shift;

                await _dbContext.SaveChangesAsync();

            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(UpdateUserViewModel updateUserRequest)
        {
            var user = await _dbContext.Users.FindAsync(updateUserRequest.Id);

            if (user != null)
            {
                user.IsActive = false;
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

		[HttpGet]
        public async Task<IActionResult> Graphs()
        {
            var allLogs = await _dbContext.Logs.Include("User").OrderBy(x => x.User.Username).GroupBy(y=>y.User.Username).ToListAsync();
			var failLogs = await _dbContext.Logs.Include("User").Where(x => !x.OnTime).OrderBy(y=>y.User.Username).GroupBy(z => z.User.Username).ToListAsync();
			//logs[0].ToArray()[0].DateTime;
			


            ViewBag.test = allLogs[0].Count();
			ViewBag.test2 = failLogs[0].Count();
			ViewBag.test3 = allLogs[0].Key;


			return View();
        }
    }
}

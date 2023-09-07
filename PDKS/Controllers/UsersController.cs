using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PDKS.Data;
using PDKS.Models;

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
                
                var new_user = new User
                {
                    Id = Guid.NewGuid(),
                    Username = addUserRequest.Username,
                    Password = addUserRequest.Password,
                };

                await _dbContext.Users.AddAsync(new_user);
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
                };

                return View(viewModel);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateUserViewModel updateUserRequest)
        {
            var user = await _dbContext.Users.FindAsync(updateUserRequest.Id);

            if (user != null)
            {
                user.Username = updateUserRequest.Username;
                user.Password = updateUserRequest.Password;
                user.IsActive = updateUserRequest.IsActive;

                await _dbContext.SaveChangesAsync();

            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(UpdateUserViewModel updateUserRequest)
        {
            var user = await _dbContext.Users.FindAsync(updateUserRequest.Id);

            if (user != null)
            {
                user.IsActive = 0;
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PDKS.Data;
using PDKS.Models;

namespace PDKS.Controllers
{
    public class LoginController : Controller
    {
        private readonly PDKSDbContext _dbContext;

        public LoginController(PDKSDbContext _dbContext)
        {
            this._dbContext = _dbContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if(HttpContext.Session.GetString("loggedUser") != null)
            {
                return RedirectToAction("Index", "Home");

            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(AddUserViewModel loginUserRequest)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Username == loginUserRequest.Username && x.Password == loginUserRequest.Password);
            if (user != null)
            {
                HttpContext.Session.SetString("loggedUser", user.Username);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["loginError"] = "User is not exist.";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("loggedUser");
            return RedirectToAction("Index");
        }
    }
}

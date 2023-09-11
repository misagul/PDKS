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
        public async Task<IActionResult> Logs()
        {
            var logs = await _dbContext.Logs.Include("User").OrderBy(x => x.User.Username).GroupBy(y=>y.User.Username).ToListAsync();
			//var failLogs = await _dbContext.Logs.Include("User").Where(x => !x.OnTime).OrderBy(y=>y.User.Username).GroupBy(z => z.User.Username).ToListAsync();
			//ViewBag.logs = logs[0].ToArray()[0].UserId;

			List<UsersLogsViewModel> usersLogs = new List<UsersLogsViewModel>();
            foreach (var log in logs)
            {
                UsersLogsViewModel newLog = new UsersLogsViewModel
                {
                    Username = log.Key,
                    Logs = log.OrderBy(x => x.Shift).ToList()
                };
				usersLogs.Add(newLog);
			}

			return View(usersLogs);
        }

        [HttpGet]
        public IActionResult Charts()
        {
            return View();
        }

		public JsonResult GetChartData()
		{
			var logs =  _dbContext.Logs.Include("User").OrderBy(x => x.User.Username).GroupBy(y => y.User.Username).ToList();
			
            ChartDataModel _chart = new ChartDataModel();
            List<string> labels = new List<string>();
            List<ChartDataModel.Datasets> _dataSet = new List<ChartDataModel.Datasets>();

            ChartDataModel.Datasets successData = new ChartDataModel.Datasets();
            successData.data = new List<int>();
            successData.label = "OnTime";
            successData.backgroundColor = "#00ff00";
            foreach (var log in logs)
            {
                labels.Add(log.Key);

                successData.data.Add(log.Where(x => x.OnTime).Count());
            }
            _dataSet.Add(successData);
            
            ChartDataModel.Datasets failData = new ChartDataModel.Datasets();
            failData.data = new List<int>();
            failData.label = "NotOnTime";
            failData.backgroundColor = "#ff0000";
            foreach (var log in logs)
            {
                failData.data.Add(log.Where(x => !x.OnTime).Count());
            }

            _dataSet.Add(failData);
            _chart.datasets = _dataSet;
            _chart.labels = labels;
            return Json(_chart);

        }
	}
}

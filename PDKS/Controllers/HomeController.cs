﻿using Microsoft.AspNetCore.Mvc;
using PDKS.Models;
using System.Diagnostics;

namespace PDKS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.loggedUser = HttpContext.Session.GetString("loggedUser");
            ViewBag.pressStatus = HttpContext.Session.GetString("pressStatus");
            ViewBag.curShift = HttpContext.Session.GetString("curShift");
            HttpContext.Session.Remove("pressStatus");
            
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
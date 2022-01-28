using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineTicketing.Data;
using OnlineTicketing.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace OnlineTicketing.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _db;
        public HomeController(ILogger<HomeController> logger,AppDbContext db)
        {
            _logger = logger;
            _db = db;
        }
        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            List<Movie> data = _db.Movies.Include(data => data.Category).OrderByDescending(data => data.Id).ToList();
            PagedList<Movie> model = new PagedList<Movie>(data, page, pageSize);
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

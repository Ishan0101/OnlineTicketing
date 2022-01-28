using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineTicketing.Data;
using OnlineTicketing.Models;
using OnlineTicketing.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineTicketing.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        public OrderController(AppDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Book(Order order)
        {
            List<Movie> movies = HttpContext.Session.Get<List<Movie>>("movies");
            if(movies!= null)
            {
                foreach(var movie in movies)
                {
                    OrderDetails orderDetails = new OrderDetails();
                    orderDetails.MovieId = movie.Id;
                    order.OrderDetails.Add(orderDetails);
                }
            }
            order.OrderNo = GetOrderNo();
            order.UserId = _userManager.GetUserId(HttpContext.User);
            order.UserName = _userManager.GetUserName(HttpContext.User);
            order.OrderDate = DateTime.Now;
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
            HttpContext.Session.Remove("movies");
            return View();

        }

        public string GetOrderNo()
        {
            int rowCount = _db.Orders.ToList().Count()+1;
            return rowCount.ToString("000");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ListOrder()
        {
            List<OrderDetails> order = _db.OrderDetails.Include(data => data.Order).Include(data=>data.Movie).OrderByDescending(data => data.Id).ToList();
            return View(order);
        }

        [Authorize(Roles = "Customer")]
        public IActionResult UserOrder()
        {
            var userid = _userManager.GetUserId(HttpContext.User);
            List<OrderDetails> order = _db.OrderDetails.Include(data => data.Order).Include(data => data.Movie).Where(data=>data.Order.UserId==userid).OrderByDescending(data => data.Id).ToList();
            return View(order);
        }
    }
}

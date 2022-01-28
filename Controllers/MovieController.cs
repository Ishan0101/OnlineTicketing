using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineTicketing.Data;
using OnlineTicketing.Models;
using X.PagedList;
using X.PagedList.Mvc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using OnlineTicketing.Utilities;

namespace OnlineTicketing.Controllers
{
    public class MovieController : Controller
    {
        private readonly AppDbContext _db;
        public MovieController(AppDbContext db)
        {
            _db = db;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult List(int page=1, int pageSize=10)
        {
            List<Movie> data = _db.Movies.Include(data => data.Category).OrderByDescending(data =>data.Id).ToList();
            PagedList<Movie> model = new PagedList<Movie>(data, page, pageSize);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(_db.Categories,"CategoryId", "CategoryName");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Movie movie)
        {
            _db.Movies.Add(movie);
            _db.SaveChanges();
            return RedirectToAction("List");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            Movie movie = _db.Movies.Find(id);
            ViewBag.CategoryId = new SelectList(_db.Categories, "CategoryId", "CategoryName");
            return View(movie);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(Movie movie, int id)
        {
            Movie m = _db.Movies.Find(id);
            m.Name = movie.Name;
            m.ShortDesc = movie.ShortDesc;
            m.Description = movie.Description;
            m.Runtime = movie.Runtime;
            m.Price = movie.Price;
            m.ImageUrl = movie.ImageUrl;
            m.StartDate = movie.StartDate;
            m.EndDate = movie.EndDate;
            m.Cast = movie.Cast;
            m.CategoryId = movie.CategoryId;
            _db.SaveChanges();
            return RedirectToAction("List");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            _db.Movies.Remove(_db.Movies.Find(id));
            _db.SaveChanges();
            return RedirectToAction("List");
        }

        public IActionResult Detail(int id)
        {
            Movie movie = _db.Movies.Find(id);
            return View(movie);
        }

        [HttpPost]
        [Authorize(Roles ="Customer")]
        public IActionResult Detail(int? id)
        {
            List<Movie> movies = new List<Movie>();
            if (id == null)
            {
                return NotFound();
            }
            var movie = _db.Movies.FirstOrDefault(c => c.Id == id);
            if (movie == null)
            {
                return NotFound();
            }
            movies = HttpContext.Session.Get<List<Movie>>("movies");
            if(movies == null)
            {
                movies = new List<Movie>();
            }
            movies.Add(movie);
            HttpContext.Session.Set("movies", movies);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public IActionResult RemoveFromCart(int? id)
        {
            List<Movie> movies = HttpContext.Session.Get<List<Movie>>("movies");
            if (movies != null)
            {
                var movie = movies.FirstOrDefault(c => c.Id == id);
                if (movie != null)
                {
                    movies.Remove(movie);
                    HttpContext.Session.Set("movies", movies);
                }
            }
            return RedirectToAction("Index","Home");
        }

        [Authorize(Roles = "Customer")]
        public IActionResult Cart()
        {
            List<Movie> movies = HttpContext.Session.Get<List<Movie>>("movies");
            if (movies==null)
            {
                movies = new List<Movie>();
            }
            return View(movies);
        }

        public IActionResult Search(string search)
        {
            List<Movie> data = _db.Movies.Include(data => data.Category).OrderByDescending(data => data.Id).ToList();
            if (!string.IsNullOrEmpty(search))
            {
                List<Movie> filtered = data.Where(n => n.Name.Contains(search,StringComparison.CurrentCultureIgnoreCase)).ToList();
                return View(filtered);
            }
            return View(data);
        }

        
    }
}

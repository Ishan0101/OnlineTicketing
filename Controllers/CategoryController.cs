using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineTicketing.Data;
using OnlineTicketing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineTicketing.Controllers
{
    [Authorize(Roles="Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _db;
        public CategoryController(AppDbContext db)
        {
            _db = db;
        }
        public IActionResult List()
        {
            var data = _db.Categories.ToList();
            return View(data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            _db.Categories.Add(category);
            _db.SaveChanges();
            return RedirectToAction("List");
        }
        public IActionResult Edit(int id)
        {
            Category category = _db.Categories.Find(id);
            return View(category);
        }
        [HttpPost]
        public IActionResult Edit(Category category, int id)
        {
            Category c = _db.Categories.Find(id);
            c.CategoryName = category.CategoryName;
            _db.SaveChanges();
            return RedirectToAction("List");
        }
        public IActionResult Delete(int CategoryId)
        {
            _db.Categories.Remove(_db.Categories.Find(CategoryId));
            _db.SaveChanges();
            return Json(true);
            
        }

    }
}

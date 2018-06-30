using Microsoft.AspNetCore.Mvc;
using CheeseMVC.Models;
using System.Collections.Generic;
using CheeseMVC.ViewModels;
using CheeseMVC.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CheeseMVC.Controllers
{
    public class CheeseController : Controller
    {
        private CheeseDbContext context;

        public CheeseController(CheeseDbContext dbContext)
        {
            context = dbContext;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            IList<Cheese> cheeses = context.Cheeses.Include(c => c.Category).ToList();
            ViewBag.Title = "All Cheeses";
            return View(cheeses);
        }

        public IActionResult Add()
        {
            IEnumerable<CheeseCategory> categories = context.Categories.ToList();
            AddCheeseViewModel addCheeseViewModel = new AddCheeseViewModel(categories);
            ViewBag.Title = "Add Cheese";
            return View(addCheeseViewModel);
        }

        [HttpPost]
        public IActionResult Add(AddCheeseViewModel addCheeseViewModel)
        {
            if (ModelState.IsValid)
            {
                // Add the new cheese to my existing cheeses
                CheeseCategory newCheeseCategory = context.Categories.Single(c => c.ID == addCheeseViewModel.CategoryID);

                Cheese newCheese = new Cheese
                {
                    Name = addCheeseViewModel.Name,
                    Description = addCheeseViewModel.Description,
                    Category = newCheeseCategory
                };

                context.Cheeses.Add(newCheese);
                context.SaveChanges();

                return Redirect("/Cheese");
            }

            return View(addCheeseViewModel);
        }

        public IActionResult Remove()
        {
            ViewBag.title = "Remove Cheeses";
            ViewBag.cheeses = context.Cheeses.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Remove(int[] cheeseIds)
        {
            foreach (int cheeseId in cheeseIds)
            {
                Cheese theCheese = context.Cheeses.Single(c => c.ID == cheeseId);
                context.Cheeses.Remove(theCheese);
            }

            context.SaveChanges();

            return Redirect("/");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            IEnumerable<CheeseCategory> categories = context.Categories.ToList();
            Cheese cheese = context.Cheeses.Single(chs => chs.ID == id);

            EditCheeseViewModel editCheeseViewModel = new EditCheeseViewModel(categories, cheese);
            return View(editCheeseViewModel);
        }

        [HttpPost]
        public IActionResult Edit(EditCheeseViewModel editCheeseViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editCheeseViewModel);
            }

            // Get the cheese that we are modifying
            Cheese changedCheese = context.Cheeses.Single(chs => chs.ID == editCheeseViewModel.CheeseID);

            // Apply modifications to the object and save
            changedCheese.Name = editCheeseViewModel.Name;
            changedCheese.Description = editCheeseViewModel.Description;
            changedCheese.CategoryID = editCheeseViewModel.CategoryID;

            context.SaveChanges();

            return Redirect("/Cheese");
        }
    }
}

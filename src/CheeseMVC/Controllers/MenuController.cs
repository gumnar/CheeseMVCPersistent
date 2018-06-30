using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CheeseMVC.Data;
using CheeseMVC.Models;
using CheeseMVC.ViewModels;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CheeseMVC.Controllers
{
    public class MenuController : Controller
    {

        private CheeseDbContext context;

        public MenuController(CheeseDbContext dbContext)
        {
            context = dbContext;
        }

        public IActionResult Index()
        {
            IList<Menu> menus = context.Menus.ToList();
            ViewBag.Title = "All Menus";
            return View(menus);
        }

        [HttpGet]
        public IActionResult Add()
        {
            AddMenuViewModel addMenuViewModel = new AddMenuViewModel();
            return View(addMenuViewModel);
        }

        [HttpPost]
        public IActionResult Add(AddMenuViewModel addMenuViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(addMenuViewModel);
            }

            Menu newMenu = new Menu
            {
                Name = addMenuViewModel.Name
            };

            context.Menus.Add(newMenu);
            context.SaveChanges();

            return Redirect("/Menu/ViewMenu/" + newMenu.ID);
        }

        [HttpGet]
        public IActionResult ViewMenu(int id)
        {
            // Get the Menu that corresponds with the given ID
            Menu menu = context.Menus.Single(m => m.ID == id);

            // Get the Items associated with the Menu
            List <CheeseMenu> items = context
                .CheeseMenus
                .Include(item => item.Cheese)
                .Where(cm => cm.CheeseID == id)
                .ToList();

            ViewMenuViewModel viewMenuViewModel = new ViewMenuViewModel
            {
                Menu = menu,
                Items = items
            };

            return View(viewMenuViewModel);
        }

        [HttpGet]
        public IActionResult AddItem(int id)
        {
            Menu menu = context.Menus.Single(m => m.ID == id);

            AddMenuItemViewModel addMenuItemViewModel = new AddMenuItemViewModel(menu, context.Cheeses);

            return View(addMenuItemViewModel);
        }

        [HttpPost]
        public IActionResult AddItem(AddMenuItemViewModel addMenuItemViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(addMenuItemViewModel);
            }

            // Look for existing CheeseMenu objects that contain the given, primary key, CheeseID and MenuID pair (composite key)
            // If the list is empty then the new relationship can be created without SQL Errors
            // Meaning: The cheese object can be added to the given menu
            IList<CheeseMenu> existingItems = context.CheeseMenus
                .Where(cm => cm.CheeseID == addMenuItemViewModel.CheeseID)
                .Where(cm => cm.MenuID == addMenuItemViewModel.MenuID)
                .ToList();

            if (existingItems.Count != 0)
            {
                // TODO : Throw an error indicating that this relationship already exists
                return RedirectToAction("AddItem", addMenuItemViewModel.MenuID);
            }

            CheeseMenu newCheeseMenu = new CheeseMenu
            {
                CheeseID = addMenuItemViewModel.CheeseID,
                MenuID = addMenuItemViewModel.MenuID
            };

            context.CheeseMenus.Add(newCheeseMenu);
            context.SaveChanges();

            return Redirect("/Menu/ViewMenu/" + addMenuItemViewModel.MenuID);
        }
    }
}

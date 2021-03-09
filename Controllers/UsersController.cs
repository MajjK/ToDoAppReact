﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoApp.Models;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace ToDoApp.Controllers
{
    public class UsersController : Controller
    {
        private ToDoDatabaseContext DbContext;

        public UsersController(ToDoDatabaseContext context)
        {
            DbContext = context;
        }

        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewData["LoginSortParm"] = String.IsNullOrEmpty(sortOrder) ? "login" : "";
            ViewData["DateSortParm"] = sortOrder == "date" ? "date_desc" : "date";
            ViewData["TasksSortParm"] = sortOrder == "tasks" ? "tasks_desc" : "tasks";
            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;
            ViewData["CurrentFilter"] = searchString;

            var users = this.GetSortedUsers(sortOrder, searchString);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(await users.Include(s => s.Tasks).AsNoTracking().ToPagedListAsync(pageNumber, pageSize));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Login, Password")] User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    DbContext.Add(user);
                    await DbContext.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            return View(user);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await DbContext.Users
                .Include(s => s.Tasks)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await DbContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userToUpdate = await DbContext.Users.FirstOrDefaultAsync(s => s.UserId == id);
            if (await TryUpdateModelAsync<User>(userToUpdate, "",
                s => s.Login, s => s.Password, s => s.AdditionDate))
            {
                try
                {
                    await DbContext.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            return View(userToUpdate);
        }

        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await DbContext.Users.Include(s => s.Tasks).AsNoTracking().FirstOrDefaultAsync(s => s.UserId == id);
            if (user == null)
            {
                return NotFound();
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await DbContext.Users.FindAsync(id);
            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                DbContext.Users.Remove(user);
                await DbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }

        private IQueryable<User> GetSortedUsers(string sortOrder, string searchString)
        {
            var users = from s in DbContext.Users
                        select s;
            if (!String.IsNullOrEmpty(searchString))
                users = users.Where(s => s.Login.Contains(searchString));
            switch (sortOrder)
            {
                case "login":
                    users = users.OrderBy(s => s.Login);
                    break;
                case "date":
                    users = users.OrderBy(s => s.AdditionDate);
                    break;
                case "date_desc":
                    users = users.OrderByDescending(s => s.AdditionDate);
                    break;
                case "tasks":
                    users = users.OrderBy(s => s.Tasks.Count);
                    break;
                case "tasks_desc":
                    users = users.OrderByDescending(s => s.Tasks.Count);
                    break;
                default:
                    users = users.OrderByDescending(s => s.Login);
                    break;
            }
            return users;
        }
    }
}

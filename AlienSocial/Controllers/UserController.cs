using AlienSocial.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AlienSocial.Controllers
{
    public class UserController : Controller
    {
        private static int _id = default;
        private bool hasAccess() => Globals.CurrentlyLoggedUser != null && Globals.Admins.Contains(Globals.CurrentlyLoggedUser.Login);

        // GET: UserController
        public ActionResult Index()
        {
            if (hasAccess()) ViewBag.admin = true;
            else ViewBag.admin = false;

            return View();
        }

        // GET: UserController/Init
        public ActionResult Init()
        {
            if (!hasAccess()) return RedirectToAction("Login", "Login");

            // add 2 first users with proper friends
            Globals.Users.Add(new User($"user_{_id++}", DateTime.Now, new Friends(new List<string>() { $"Alien", $"Furkacz" })));
            Globals.Users.Add(new User($"user_{_id++}", DateTime.Now, new Friends(new List<string>() { $"Kaczek", $"Furkacz" })));

            // add the rest of the users
            for (int i = 0; i < 4; i++)
            {
                Globals.Users.Add(new User($"user_{_id++}", DateTime.Now, new Friends(new List<string>() { $"user_{_id - 1}", $"user_{_id - 2}" })));
            }

            return RedirectToAction("List", "User");
        }

        // GET: UserController/List
        public ActionResult List()
        {
            if (!hasAccess()) return RedirectToAction("Login", "Login");

            return View(Globals.Users);
        }

        // GET: UserController/Add
        public ActionResult Add()
        {
            if (!hasAccess()) return RedirectToAction("Login", "Login");

            return View();
        }

        // POST: UserController/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(string login)
        {
            if (!hasAccess()) return RedirectToAction("Login", "Login");
            
            if(!ModelState.IsValid)  return View(login);

            // check if the user already exists
            if (!Globals.Users.Any(x => x.Login == login))
            {
                Globals.Users.Add(new User(login, DateTime.Now, new Friends(new List<string>())));
                
            }

            return RedirectToAction("List", "User");

        }

        // GET: UserController/Delete/5
        public ActionResult Delete(string login)
        {
            if (!hasAccess()) return RedirectToAction("Login", "Login");

            if (login == null) return RedirectToAction("List", "User");

            ViewBag.Login = login;
            return View();
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string login, IFormCollection collection)
        {
            // try to delete the user
            try
            {
                Globals.Users.RemoveAll(u => u.Login == login);
                return RedirectToAction("List", "User");
            }
            catch
            {
                return View();
            }
        }
    }
}

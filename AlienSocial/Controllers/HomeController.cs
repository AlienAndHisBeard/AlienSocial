using AlienSocial.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AlienSocial.Controllers
{
    public class HomeController : Controller
    {
        private bool LoggedIn() => Globals.CurrentlyLoggedUser != null;
        private bool HasAccess() => LoggedIn() && Globals.Admins.Contains(Globals.CurrentlyLoggedUser.Login);
        public IActionResult Index()
        {
            // navbar values
            if (LoggedIn())
            {
                ViewBag.LoggedIn = true;
                if (HasAccess()) ViewBag.admin = true;
                else ViewBag.admin = false;
            } else ViewBag.LoggedIn = false;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
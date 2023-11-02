using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AlienSocial.Controllers
{
    public class LoginController : Controller
    {
        private bool LoggedIn() => Globals.CurrentlyLoggedUser != null;
        // is the user an admin
        private bool HasAccess() => LoggedIn() && Globals.Admins.Contains(Globals.CurrentlyLoggedUser.Login);

        // GET: LoginController
        public ActionResult Index()
        {
            // navbar values
            if (LoggedIn())
            {
                ViewBag.LoggedIn = true;
                if (HasAccess()) ViewBag.admin = true;
                else ViewBag.admin = false;
            }
            else ViewBag.LoggedIn = false;

            return View();
        }

        public ActionResult Login()
        {
            // navbar values
            if (LoggedIn())
            {
                ViewBag.LoggedIn = true;
                if (HasAccess()) ViewBag.admin = true;
                else ViewBag.admin = false;
            }
            else ViewBag.LoggedIn = false;

            return View();
        }

        public ActionResult Logout()
        {
            Globals.CurrentlyLoggedUser = null;
            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string login)
        {
            var currentlyLoggedUser = Globals.Users.FirstOrDefault(x => x.Login == login);

            if (currentlyLoggedUser == null)
            {
                // navbar values
                if (LoggedIn())
                {
                    ViewBag.LoggedIn = true;
                    if (HasAccess()) ViewBag.admin = true;
                    else ViewBag.admin = false;
                }
                else ViewBag.LoggedIn = false;

                return View();
            }
            else
            {
                // log in the user
                Globals.CurrentlyLoggedUser = currentlyLoggedUser;
                return RedirectToAction("", "Friends");
            }

        }


    }
}

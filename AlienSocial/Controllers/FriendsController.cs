using AlienSocial.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.Text;
using System.Text.Json.Nodes;

namespace AlienSocial.Controllers
{
    public class FriendsController : Controller
    {
        private static bool LoggedIn() => Globals.CurrentlyLoggedUser != null;

        // check if admin
        private static bool HasAccess() => LoggedIn() && Globals.Admins.Contains(Globals.CurrentlyLoggedUser!.Login);

        public IActionResult Index()
        {
            if (!LoggedIn()) return RedirectToAction("Login", "Login");

            // save helper data for navbar
            ViewBag.LoggedIn = true;
            if (HasAccess()) ViewBag.admin = true;
            else ViewBag.admin = false;
            ViewBag.user = Globals.CurrentlyLoggedUser!.Login;

            // read and parse the data from List() action
            Friends friends;
            if (List().ToJson() == null) return View(new Friends(new List<string>()));

            try
            {
                JsonNode j = JsonNode.Parse(List().ToJson())!;
                friends = JsonConvert.DeserializeObject<Friends>(j["Value"]!.ToJsonString())!;
            }
            catch (ArgumentNullException e) { return View(new Friends(new List<string>())); }
            catch (System.Text.Json.JsonException) { return RedirectToAction("", "Home");  }

            return View(friends);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(string login)
        {
            if (!LoggedIn()) return RedirectToAction("Login", "Login");

            if (!ModelState.IsValid) return Json(new { success = false });

            // check if the friend is not already in the friendlist
            if (Globals.CurrentlyLoggedUser!.Friends.friends.Contains(login)) return Json(new { success = true });

            // check if the user exists
            if(Globals.Users.Any(x => x.Login == login))
            {
                Globals.CurrentlyLoggedUser?.Friends.friends.Add(login);
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        public ActionResult List()
        {
            if (!LoggedIn()) return RedirectToAction("Login", "Login");

            return Json(new { Globals.CurrentlyLoggedUser?.Friends.friends });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string login, IFormCollection collection)
        {
            if (!LoggedIn()) return RedirectToAction("Login", "Login");

            bool result;
            // try to delete
            try
            {
                Globals.CurrentlyLoggedUser?.Friends.friends.RemoveAll(u => u == login);
                result = true;
            }
            catch
            {
                result = false;
            }

            return Json(new { success = result });
        }

        public ActionResult Export()
        {
            if (!LoggedIn()) return RedirectToAction("Login", "Login");

            // check if there are any friends to export
            var friends = Globals.CurrentlyLoggedUser?.Friends;

            if (friends is null || friends.friends.Count == 0)
            {
                return RedirectToAction("", "Friends");
            }

            // create filename
            string filePath = $"{Globals.CurrentlyLoggedUser!.Login}_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt";

            // create the data to save
            StringBuilder sb = new();
            Globals.CurrentlyLoggedUser!.Friends.friends.ForEach(f => sb.AppendLine(f));
            byte[] fileContent = Encoding.ASCII.GetBytes(sb.ToString());

            // download the data
            return new FileContentResult(fileContent, "text/txt")
            {
                FileDownloadName = filePath
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(IFormFile postedFile)
        {
            if (!LoggedIn()) return RedirectToAction("Login", "Login");

            // simple validation of the file 
            if (postedFile == null) return RedirectToAction("", "Friends");
            if (postedFile.ContentType != "text/plain") return RedirectToAction("", "Friends");

            // clear previous friendlist and add one read from the file
            Globals.CurrentlyLoggedUser?.Friends.friends.Clear();
            Globals.CurrentlyLoggedUser?.Friends.friends.AddRange(RetriveFriendsFromFile(postedFile));

            return RedirectToAction("", "Friends");
        }


        // utility function to handle names from a file
        private static List<string> RetriveFriendsFromFile(IFormFile file)
        {
            var result = new List<string>();
            string temp;
            // go through the file line by line
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                {
                    temp = reader.ReadLine() ?? "";

                    // skip if user doesn't exist
                    if (!Globals.Users.Any(x => x.Login == temp)) continue;

                    // add the friend
                    result.Add(temp);
                }
            }

            return result;
        }
    }
}

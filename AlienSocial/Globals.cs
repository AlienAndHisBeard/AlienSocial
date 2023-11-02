using AlienSocial.Models;

namespace AlienSocial
{
    public static class Globals
    {
        public static List<User> Users { get; set; } = new List<User>()
        {
            new User("Alien", DateTime.Now, new Friends(new List<string>() { "Furkacz", "Kaczek" })),
            new User("Kaczek", DateTime.Now, new Friends(new List<string>() { "Alien", "Furkacz" })),
            new User("Furkacz", DateTime.Now, new Friends(new List<string>() { "Alien", "Kaczek" }))
        };
        public static List<string> Admins { get; set; } = new List<string>() { "Alien" };
        public static User? CurrentlyLoggedUser = null;
    }
}

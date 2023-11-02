namespace AlienSocial.Models
{
    public class User
    {
        public string Login { get; set; }
        public DateTime CreatedAt { get; set; }
        public Friends Friends { get; set; }
        public User( string login, DateTime createdAt, Friends friends) 
        {
            Login = login;
            CreatedAt = createdAt;
            Friends = friends;
        }
    }
}

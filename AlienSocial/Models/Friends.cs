using Newtonsoft.Json;

namespace AlienSocial.Models
{
    public class Friends
    {
        public List<string> friends { get; set; }

        [JsonConstructor]
        public Friends(string[] friends)
        {
            if(friends == null)
                this.friends = new List<string>();
            else
                this.friends = friends.ToList<string>();
        }

        public Friends(List<string> friends)
        {
            this.friends = friends;
        }
    }
}


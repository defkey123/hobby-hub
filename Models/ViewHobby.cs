namespace BeltExam3HobbyHub.Models
{
    public class ViewHobby
    {
        public Hobby Hobby { get; set; }
        public HobbyJoined HobbyJoined { get; set; }
        public User CurrentUser { get; set; }
    }
}
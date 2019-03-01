using System.ComponentModel.DataAnnotations;

namespace BeltExam3HobbyHub.Models
{
    public class HobbyJoined
    {
        public int HobbyJoinedId { get; set; }

        public int HobbyId { get; set; }
        public Hobby Hobby { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public string Proficiency { get; set; }
    }
}
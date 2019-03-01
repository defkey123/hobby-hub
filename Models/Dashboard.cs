using System.Collections.Generic;

namespace BeltExam3HobbyHub.Models
{
    public class Dashboard
    {
        public User CurrentUser { get; set; }
        public List<Hobby> Hobbies { get; set; }

        public List<Hobby> TopNovice { get; set; }
        public List<Hobby> TopIntermediate { get; set; }
        public List<Hobby> TopExpert { get; set; }
    }
}
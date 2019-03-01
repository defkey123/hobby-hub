using System;
using Microsoft.EntityFrameworkCore;

namespace BeltExam3HobbyHub.Models
{
    public class HobbiesContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public HobbiesContext(DbContextOptions<HobbiesContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Hobby> Hobbies { get; set; }
        public DbSet<HobbyJoined> HobbiesJoined { get; set; }
    }
}

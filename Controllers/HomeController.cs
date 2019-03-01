using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BeltExam3HobbyHub.Models;


namespace BeltExam3HobbyHub.Controllers
{
    public class HomeController : Controller
    {
        private HobbiesContext dbContext;
        public HomeController(HobbiesContext context)
        {
            dbContext = context;
        }
        [HttpGet("")]
        public IActionResult Index()
        {
            return RedirectToAction("Hobbies");
        }

        [HttpGet("login")]
        public IActionResult LoginReg()
        {
            int? userId = HttpContext.Session.GetInt32("logged_in_id");
            if (userId != null) return RedirectToAction("Hobbies");

            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            // Check initial ModelState
            if (ModelState.IsValid)
            {
                // If a User exists with provided Username
                if (dbContext.Users.Any(u => u.Username == user.Username))
                {
                    // Manually add a ModelState error to the Username field, with provided
                    // error message
                    ModelState.AddModelError("Username", "Username already in use!");

                    return View("LoginReg");
                }
                PasswordHasher<User> hasher = new PasswordHasher<User>();

                user.Password = hasher.HashPassword(user, user.Password);
                dbContext.Users.Add(user);
                dbContext.SaveChanges();
                HttpContext.Session.SetInt32("logged_in_id", user.UserId);
                return RedirectToAction("Hobbies");
            }
            return View("LoginReg");
        }

        [HttpPost("confirmlogin")]
        public IActionResult Login(LoginUser userSubmission)
        {
            if (ModelState.IsValid)
            {
                // If inital ModelState is valid, query for a user with provided Username
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Username == userSubmission.LoginUsername);
                // If no user exists with provided Username
                if (userInDb is null)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("LoginUsername", "Invalid Username/Password");
                    return View("LoginReg");
                }

                // Initialize hasher object
                var hasher = new PasswordHasher<LoginUser>();

                // varify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.LoginPassword);

                // result can be compared to 0 for failure
                if (result == 0)
                {
                    return View("LoginReg");
                }
                else
                {
                    HttpContext.Session.SetInt32("logged_in_id", userInDb.UserId);
                    return RedirectToAction("Hobbies");
                }
            }
            return View("LoginReg");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Hobbies");
        }

        [HttpGet("home")]
        public IActionResult Hobbies()
        {
            int? userId = HttpContext.Session.GetInt32("logged_in_id");
            if (userId is null) return RedirectToAction("LoginReg");

            var Hobbies = dbContext.Hobbies.ToList();
            Dashboard dash = new Dashboard();
            dash.Hobbies = dbContext.Hobbies
                .Include(a => a.UsersJoined)
                    .ThenInclude(b => b.User)
                .ToList();

            dash.TopNovice = dbContext.Hobbies
                .Include(a => a.UsersJoined)
                    .Where(h => h.UsersJoined.Any(u => u.Proficiency == "Novice"))
                    .OrderByDescending(x => x.UsersJoined.Count)
                    .Take(3)
                .ToList();
            
            dash.TopIntermediate = dbContext.Hobbies
                .Include(a => a.UsersJoined)
                    .Where(h => h.UsersJoined.Any(u => u.Proficiency == "Intermediate"))
                    .OrderByDescending(x => x.UsersJoined.Count)
                    .Take(3)
                .ToList();

            dash.TopExpert = dbContext.Hobbies
                .Include(a => a.UsersJoined)
                    .Where(h => h.UsersJoined.Any(u => u.Proficiency == "Expert"))
                    .OrderByDescending(x => x.UsersJoined.Count)
                    .Take(3)
                .ToList();

            dash.CurrentUser = dbContext.Users.FirstOrDefault(u => u.UserId == (int)userId);



            return View(dash);
        }

        [HttpGet("new")]
        public IActionResult NewHobby()
        {
            int? userId = HttpContext.Session.GetInt32("logged_in_id");
            if (userId is null) return RedirectToAction("LoginReg");

            return View();
        }

        [HttpPost("post_new_hobby")]
        public IActionResult PostNewHobby(Hobby hobby)
        {
            int? userId = HttpContext.Session.GetInt32("logged_in_id");
            if (userId is null) return RedirectToAction("LoginReg");

            if (ModelState.IsValid)
            {
                dbContext.Hobbies.Add(hobby);
                dbContext.SaveChanges();
                return RedirectToAction("Hobbies");
            }
            return View("NewHobby");
        }

        [HttpGet("hobby/edit/{hobbyId}")]
        public IActionResult EditHobby(int hobbyId)
        {
            int? userId = HttpContext.Session.GetInt32("logged_in_id");
            if (userId is null) return RedirectToAction("LoginReg");

            Hobby hobby = dbContext.Hobbies
                .Include(x => x.UsersJoined)
                    .ThenInclude(x => x.User)
                .FirstOrDefault(h => h.HobbyId == hobbyId);

            return View(hobby);
        }

        [HttpPost("hobby/edit/{hobbyId}/post")]
        public IActionResult PostEditHobby(int hobbyId, Hobby hobby)
        {
            int? userId = HttpContext.Session.GetInt32("logged_in_id");
            if (userId is null) return RedirectToAction("LoginReg");

            if(ModelState.IsValid)
            {
                Hobby editedHobby = dbContext.Hobbies.FirstOrDefault(h => h.HobbyId == hobbyId);
                editedHobby.Name = hobby.Name;
                editedHobby.Description = hobby.Description;
                dbContext.SaveChanges();
                return RedirectToAction("Hobbies");
            }

            return View("EditHobby");
        }

        [HttpGet("hobby/{hobbyId}")]
        public IActionResult ViewHobby(int hobbyId)
        {
            int? userId = HttpContext.Session.GetInt32("logged_in_id");
            if (userId is null) return RedirectToAction("LoginReg");

            Hobby hobby = dbContext.Hobbies
                .Include(x => x.UsersJoined)
                    .ThenInclude(x => x.User)
                .FirstOrDefault(h => h.HobbyId == hobbyId);

            ViewHobby hobbyInfo = new ViewHobby();
            hobbyInfo.CurrentUser = dbContext.Users.FirstOrDefault(u => u.UserId == userId);
            hobbyInfo.Hobby = hobby;
            hobbyInfo.HobbyJoined = new HobbyJoined();

            return View(hobbyInfo);
        }

        [HttpPost("hobby/{hobbyId}/join")]
        public IActionResult JoinHobby(int hobbyId, HobbyJoined newHobbyJoin)
        {
            int? userId = HttpContext.Session.GetInt32("logged_in_id");
            if (userId is null) return RedirectToAction("LoginReg");

            newHobbyJoin.UserId = (int)userId;
            newHobbyJoin.HobbyId = hobbyId;
            dbContext.HobbiesJoined.Add(newHobbyJoin);
            dbContext.SaveChanges();

            return RedirectToAction("Hobbies");
        }


    }
}

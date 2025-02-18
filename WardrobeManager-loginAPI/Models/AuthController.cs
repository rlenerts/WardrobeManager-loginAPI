namespace WardrobeManager_loginAPI.Models
{
    using Microsoft.AspNetCore.Mvc;
    using System.Linq;

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private static List<User> Users = new List<User>(); 

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterModel model)
        {
            
            if (Users.Any(u => u.Username == model.Username))
            {
                return BadRequest("User already exists.");
            }

            string role = model.IsAdmin ? "Admin" : "Regular";

            string passwordHash = PasswordService.HashPassword(model.Password);
            var user = new User
            {
                Username = model.Username,
                PasswordHash = passwordHash,
                Role = role 
            };

            Users.Add(user);
            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            var user = Users.FirstOrDefault(u => u.Username == model.Username);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            string hashedPassword = PasswordService.HashPassword(model.Password);
            if (hashedPassword != user.PasswordHash)
            {
                return Unauthorized("Invalid password.");
            }

            return Ok(new { Message = "Login successful", Role = user.Role });
        }
    }

    public class RegisterModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }

    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

}

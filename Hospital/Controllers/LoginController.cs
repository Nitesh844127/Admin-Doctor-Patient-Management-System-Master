using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.Models;
using System.Data.SQLite;
using System.Security.Claims;
using Dapper;

namespace Hospital.Controllers
{
    public class LoginController : Controller
    {

        private IConfiguration Configuration;
        private readonly ILogger<LoginController> logger;
        private readonly IWebHostEnvironment Environment;

        public LoginController(ILogger<LoginController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
        {
            logger = _logger;
            Configuration = _Configuration;
            Environment = _environment;
        }
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous] //for new user access
        [HttpPost]
        public IActionResult Index(string username, string password)
        {
            string Conn = this.Configuration.GetConnectionString("myconn");
            using (SQLiteConnection db = new SQLiteConnection(Conn))
            {
                User us = new User();
                ClaimsIdentity identity = null;
                int checker = db.ExecuteScalar<int>("select count(*)  from User where username='" + username + "' and password='" + password + "'");
                if (checker > 0)
                {
                    //  us = db.Query<User>("select * from user where username='" + username + "' and password='" + password + "'").FirstOrDefault();
                    identity = new ClaimsIdentity(new[] {
                  new Claim(ClaimTypes.Name, username)
              }, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ViewBag.msg = "User not found";
                    return View();
                }

            }


        }

       
        [Authorize] 
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }

    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data.SQLite;

namespace Hospital.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<AdminController> logger;
        private readonly IWebHostEnvironment Environment;

        public AdminController(ILogger<AdminController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
        {
            logger = _logger;
            Configuration = _Configuration;
            Environment = _environment;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}

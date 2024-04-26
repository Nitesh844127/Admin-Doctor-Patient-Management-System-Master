using Microsoft.AspNetCore.Mvc;
using System.Data.SQLite;
using Dapper;
using Hospital.Models;

namespace Hospital.Controllers
{
    public class CityController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<CityController> logger;
        private readonly IWebHostEnvironment Environment;

        public CityController(ILogger<CityController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
        {
            logger = _logger;
            Configuration = _Configuration;
            Environment = _environment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            

            List<City> cities = new List<City>();
            string Conn = this.Configuration.GetConnectionString("myconn");
            using (SQLiteConnection db = new SQLiteConnection(Conn))
            {
                cities = db.Query<City>("select * from City" ).ToList();
            }
            return View(cities);
        }
       

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(City cities)
        {
            string Conn = this.Configuration.GetConnectionString("myconn");
            using (SQLiteConnection db = new SQLiteConnection(Conn))
            {
                cities.id = db.ExecuteScalar<int>("select Max(id) from City") + 1;
                db.Execute("insert into  City(id,cityName)values(@id,@cityName)", cities);
            }

            return RedirectToAction("Index");
        }
        
        [HttpGet]
        public IActionResult Edit(int id)
        {
            City ct = new City();
            string Conn = this.Configuration.GetConnectionString("myconn");
            using (SQLiteConnection db = new SQLiteConnection(Conn))
            {
                ct = db.Query<City>("select * from City where id=" + id).FirstOrDefault();
            }
            return View(ct);
        }

        [HttpPost]
        public IActionResult Edit(City cities)
        {
            string Conn = this.Configuration.GetConnectionString("myconn");
            using (SQLiteConnection db = new SQLiteConnection(Conn))
            {
                db.Execute("update City set cityName=@cityName where id=" + cities.id, cities);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            City ct = new City();
            string Conn = this.Configuration.GetConnectionString("myconn");
            using (SQLiteConnection db = new SQLiteConnection(Conn))
            {
                ct = db.Query<City>("select * from City where id=" + id).FirstOrDefault();
            }
            return View(ct);
        }

        [HttpPost]
        public IActionResult Delete(City cities)
        {
            string Conn = this.Configuration.GetConnectionString("myconn");
            using (SQLiteConnection db = new SQLiteConnection(Conn))
            {

                db.Execute("delete from City where id=" + cities.id);
            }

            return RedirectToAction("Index");
        }

    }
}

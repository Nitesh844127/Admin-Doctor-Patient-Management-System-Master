using Microsoft.AspNetCore.Mvc;
using Hospital.Models;
using System.Data.SQLite;
using Dapper;

namespace Hospital.Controllers
{
    public class DoctorController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<DoctorController> logger;
        private readonly IWebHostEnvironment Environment;

        public DoctorController(ILogger<DoctorController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
        {
            logger = _logger;
            Configuration = _Configuration;
            Environment = _environment;
        }

        [HttpGet]
        public IActionResult Index()
        {


            List<Doctor> doctores = new List<Doctor>();
            string Conn = this.Configuration.GetConnectionString("myconn");
            using (SQLiteConnection db = new SQLiteConnection(Conn))
            {
                doctores = db.Query<Doctor>("select * from Doctor").ToList();
            }
            return View(doctores);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(Doctor doctores)
        {
            string Conn = this.Configuration.GetConnectionString("myconn");
            using (SQLiteConnection db = new SQLiteConnection(Conn))
            {
                doctores.id = db.ExecuteScalar<int>("select Max(id) from Doctor") + 1;
                db.Execute("insert into  Doctor(id,doctorName)values(@id,@doctorName)", doctores);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Doctor ct = new Doctor();
            string Conn = this.Configuration.GetConnectionString("myconn");
            using (SQLiteConnection db = new SQLiteConnection(Conn))
            {
                ct = db.Query<Doctor>("select * from Doctor where id=" + id).FirstOrDefault();
            }
            return View(ct);
        }

        [HttpPost]
        public IActionResult Edit(Doctor doctores)
        {
            string Conn = this.Configuration.GetConnectionString("myconn");
            using (SQLiteConnection db = new SQLiteConnection(Conn))
            {
                db.Execute("update Doctor set doctorName=@doctorName where id=" + doctores.id, doctores);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            Doctor ct = new Doctor();
            string Conn = this.Configuration.GetConnectionString("myconn");
            using (SQLiteConnection db = new SQLiteConnection(Conn))
            {
                ct = db.Query<Doctor>("select * from Doctor where id=" + id).FirstOrDefault();
            }
            return View(ct);
        }

        [HttpPost]
        public IActionResult Delete(Doctor doctores)
        {
            string Conn = this.Configuration.GetConnectionString("myconn");
            using (SQLiteConnection db = new SQLiteConnection(Conn))
            {

                db.Execute("delete from Doctor where id=" + doctores.id);
            }

            return RedirectToAction("Index");
        }

    }
}

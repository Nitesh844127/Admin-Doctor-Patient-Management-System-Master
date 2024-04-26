using Microsoft.AspNetCore.Mvc;
using Hospital.Models;
using System.Data.SQLite;
using Dapper;

namespace Hospital.Controllers
{
    public class DiseaseController : Controller
    {

        private IConfiguration Configuration;
        private readonly ILogger<DiseaseController> logger;
        private readonly IWebHostEnvironment Environment;

        public DiseaseController(ILogger<DiseaseController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
        {
            logger = _logger;
            Configuration = _Configuration;
            Environment = _environment;
        }

        [HttpGet]
        public IActionResult Index()
        {


            List<Disease> diseases = new List<Disease>();
            string Conn = this.Configuration.GetConnectionString("myconn");
            using (SQLiteConnection db = new SQLiteConnection(Conn))
            {
                diseases = db.Query<Disease>("select * from Disease").ToList();
            }
            return View(diseases);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(Disease diseases)
        {
            string Conn = this.Configuration.GetConnectionString("myconn");
            using (SQLiteConnection db = new SQLiteConnection(Conn))
            {
                diseases.id = db.ExecuteScalar<int>("select Max(id) from Disease") + 1;
                db.Execute("insert into  Disease(id,diseaseName)values(@id,@diseaseName)", diseases);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Disease ct = new Disease();
            string Conn = this.Configuration.GetConnectionString("myconn");
            using (SQLiteConnection db = new SQLiteConnection(Conn))
            {
                ct = db.Query<Disease>("select * from Disease where id=" + id).FirstOrDefault();
            }
            return View(ct);
        }

        [HttpPost]
        public IActionResult Edit(Disease diseases)
        {
            string Conn = this.Configuration.GetConnectionString("myconn");
            using (SQLiteConnection db = new SQLiteConnection(Conn))
            {
                db.Execute("update Disease set diseaseName=@diseaseName where id=" + diseases.id, diseases);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            Disease ct = new Disease();
            string Conn = this.Configuration.GetConnectionString("myconn");
            using (SQLiteConnection db = new SQLiteConnection(Conn))
            {
                ct = db.Query<Disease>("select * from Disease where id=" + id).FirstOrDefault();
            }
            return View(ct);
        }

        [HttpPost]
        public IActionResult Delete(Disease diseases)
        {
            string Conn = this.Configuration.GetConnectionString("myconn");
            using (SQLiteConnection db = new SQLiteConnection(Conn))
            {

                db.Execute("delete from Disease where id=" + diseases.id);
            }

            return RedirectToAction("Index");
        }

    }
}

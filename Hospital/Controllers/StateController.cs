using Microsoft.AspNetCore.Mvc;
using Hospital.Models;
using System.Data.SQLite;
using Dapper;

namespace Hospital.Controllers
{
    public class StateController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<StateController> logger;
        private readonly IWebHostEnvironment Environment;

        public StateController(ILogger<StateController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
        {
            logger = _logger;
            Configuration = _Configuration;
            Environment = _environment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<State> states = new List<State>();
            string Conn = this.Configuration.GetConnectionString("myconn");
            using (SQLiteConnection db = new SQLiteConnection(Conn))
            {
                states = db.Query<State>("select * from State").ToList();
            }
            return View(states);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(State states)
        {
            string Conn = this.Configuration.GetConnectionString("myconn");
            using (SQLiteConnection db = new SQLiteConnection(Conn))
            {
                states.id = db.ExecuteScalar<int>("select Max(id) from State") + 1;
                db.Execute("insert into  State(id,stateName)values(@id,@stateName)", states);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            State ct = new State();
            string Conn = this.Configuration.GetConnectionString("myconn");
            using (SQLiteConnection db = new SQLiteConnection(Conn))
            {
                ct = db.Query<State>("select * from State where id=" + id).FirstOrDefault();
            }
            return View(ct);
        }

        [HttpPost]
        public IActionResult Edit(State states)
        {
            string Conn = this.Configuration.GetConnectionString("myconn");
            using (SQLiteConnection db = new SQLiteConnection(Conn))
            {
                db.Execute("update State set stateName=@stateName where id=" + states.id, states);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            State ct = new State();
            string Conn = this.Configuration.GetConnectionString("myconn");
            using (SQLiteConnection db = new SQLiteConnection(Conn))
            {
                ct = db.Query<State>("select * from State where id=" + id).FirstOrDefault();
            }
            return View(ct);
        }

        [HttpPost]
        public IActionResult Delete(State states)
        {
            string Conn = this.Configuration.GetConnectionString("myconn");
            using (SQLiteConnection db = new SQLiteConnection(Conn))
            {

                db.Execute("delete from State where id=" + states.id);
            }

            return RedirectToAction("Index");
        }

    }
}

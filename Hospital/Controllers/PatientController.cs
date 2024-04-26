using Microsoft.AspNetCore.Mvc;
using System.Data.SQLite;
using Dapper;
using Hospital.Models;
using System.Data;
using ClosedXML.Excel;
using iTextSharp.text.pdf;
using iTextSharp.text;


namespace Hospital.Controllers
{
   
    public class PatientController : Controller
    {
        private IConfiguration Configuration;
        private readonly ILogger<PatientController> logger;
        private readonly IWebHostEnvironment Environment;

        public PatientController(ILogger<PatientController> _logger, IConfiguration _Configuration, IWebHostEnvironment _environment)
        {
            logger = _logger;
            Configuration = _Configuration;
            Environment = _environment;
        }

        [HttpGet]
        public IActionResult Index(string ptName,int cityId ,int doctorId,int stateId,int diseaseId,int h)
        {
            string where = " 1=1";
            if (ptName != null)
            {
                where += " and Patient.ptName='" + ptName + "'";
            }
            if (cityId != 0)
            {
                where += " and Patient.cityId=" + cityId;
            }
            if (doctorId != 0)
            {
                where += " and Patient.doctorId=" + doctorId;
            }
            if (stateId != 0)
            {
                where += " and Patient.stateId=" + stateId;
            }
            if (diseaseId != 0)
            {
                where += " and Patient.diseaseId=" + diseaseId;
            }

            ViewBag.ptName = ptName;
            ViewBag.cityId = cityId;
            ViewBag.doctorId = doctorId;
            ViewBag.stateId = stateId;
            ViewBag.diseaseId = diseaseId;

            List<Patient> patients = new List<Patient>();
            string Conn = this.Configuration.GetConnectionString("myconn");
            using (SQLiteConnection db = new SQLiteConnection(Conn))
            {
                var patientNames = db.Query<string>("SELECT ptName FROM Patient").ToList();
                
                ViewBag.PatientNames = patientNames;
                ViewBag.cl = db.Query<Doctor>("Select * from Doctor ").ToList();
                ViewBag.ct = db.Query<City>("Select * from City ").ToList();
                ViewBag.st = db.Query<State>("Select * from State ").ToList();
                ViewBag.cs = db.Query<Disease>("Select * from Disease ").ToList();
                patients = db.Query<Patient>("select Patient.*,City.cityName as cityName ,Disease.diseaseName as diseaseName ,State.stateName as stateName ,Doctor.doctorName as doctorName " +
                    "from Patient left join City on Patient.cityId=City.id left join Disease on Patient.diseaseId=Disease.id left join State on Patient.stateId=State.id left join Doctor on Patient.doctorId=Doctor.id where " + where).ToList();
                decimal Total = patients.Sum(i => i.fees);
                ViewBag.totalFees = Total;
            }
            return View(patients);
        }

        [HttpPost]
        public IActionResult Index(string ptName, int cityId, int doctorId, int stateId, int diseaseId)
        {
            return RedirectToAction("Index", new { ptName = ptName, cityId = cityId, doctorId = doctorId, stateId = stateId, diseaseId = diseaseId });
        }


        [HttpGet]
        public IActionResult Create(string url)
        {
            string Conn = this.Configuration.GetConnectionString("myconn");
            using (SQLiteConnection db = new SQLiteConnection(Conn))
            { 
                ViewBag.url = url;
                ViewBag.cl = db.Query<Doctor>("Select * from Doctor ").ToList();
                ViewBag.ct = db.Query<City>("Select * from City ").ToList();
                ViewBag.st = db.Query<State>("Select * from State ").ToList();
                ViewBag.cs = db.Query<Disease>("Select * from Disease ").ToList();
            }
            return View();
        }


        [HttpPost]
    
        public IActionResult Create(Patient patients, IFormFile patientImage, string url)
        {
            string Conn = this.Configuration.GetConnectionString("myconn");
            using (SQLiteConnection db = new SQLiteConnection(Conn))
            {
                if (patientImage != null && patientImage.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Environment.WebRootPath, "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + patientImage.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    patientImage.CopyTo(new FileStream(filePath, FileMode.Create));
                    patients.ImagePath = "/images/" + uniqueFileName;
                }

                patients.id = db.ExecuteScalar<int>("select Max(id) from Patient") + 1;
                db.Execute("insert into  Patient(id,ptName,doctorId,cityId,stateId,mobNo,cast,diseaseId,gender,fees, ImagePath) values(@id,@ptName,@doctorId,@cityId,@stateId,@mobNo,@cast,@diseaseId,@gender,@fees, @ImagePath)", patients);
            }

            return Redirect(url);
        }

       

        [HttpGet]
        public IActionResult Edit(int id,string url)
        {
            Patient ctt = new Patient();
            string Conn = this.Configuration.GetConnectionString("myconn");
            using (SQLiteConnection db = new SQLiteConnection(Conn))
            {
                ViewBag.url = url;
                ViewBag.cl = db.Query<Doctor>("Select * from Doctor ").ToList();
                ViewBag.ct = db.Query<City>("Select * from City ").ToList();
                ViewBag.st = db.Query<State>("Select * from State ").ToList();
                ViewBag.cs = db.Query<Disease>("Select * from Disease ").ToList();
                ctt = db.Query<Patient>("select * from Patient where id=" + id).FirstOrDefault();
            }
            return View(ctt);
        }

        [HttpPost]
        public IActionResult Edit(Patient patients, IFormFile patientImage, string url)
        {
            string Conn = this.Configuration.GetConnectionString("myconn");
            using (SQLiteConnection db = new SQLiteConnection(Conn))
            {
                if (patientImage != null && patientImage.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Environment.WebRootPath, "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + patientImage.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    patientImage.CopyTo(new FileStream(filePath, FileMode.Create));
                    patients.ImagePath = "/images/" + uniqueFileName;
                }
                else
                {
                    patients.ImagePath = db.ExecuteScalar<string>("select ImagePath from Patient where id=@id", new { id = patients.id });
                }
                db.Execute("update Patient set ptName=@ptName,doctorId=@doctorId,cityId=@cityId,stateId=@stateId,mobNo=@mobNo,cast=@cast,diseaseId=@diseaseId,gender=@gender,fees=@fees, ImagePath=@ImagePath where id=@id", patients);
            }

            return Redirect(url);
        }
      
        [HttpGet]
        public IActionResult Delete(int id, string url)
        {
            Patient ct = new Patient();
            string Conn = this.Configuration.GetConnectionString("myconn");
            using (SQLiteConnection db = new SQLiteConnection(Conn))
            {
                ViewBag.url = url;
                ct = db.Query<Patient>("select * from Patient where id=" + id).FirstOrDefault();
            }
            return View(ct);
        }

        [HttpPost]
        public IActionResult Delete(Patient patients, string url)
        {
            string Conn = this.Configuration.GetConnectionString("myconn");
            using (SQLiteConnection db = new SQLiteConnection(Conn))
            {

                db.Execute("delete from Patient where id=" + patients.id);
            }

            return Redirect(url);
        }


        public FileContentResult ExcelExport(string ptName, int cityId, int doctorId, int stateId, int diseaseId)
        {
            string where = " 1=1";
            if (ptName != null)
            {
                where += " and Patient.ptName='" + ptName + "'";
            }
            if (cityId != 0)
            {
                where += " and Patient.cityId=" + cityId;
            }
            if (doctorId != 0)
            {
                where += " and Patient.doctorId=" + doctorId;
            }
            if (stateId != 0)
            {
                where += " and Patient.stateId=" + stateId;
            }
            if (diseaseId != 0)
            {
                where += " and Patient.diseaseId=" + diseaseId;
            }

            List<Patient> patients = new List<Patient>();
            string Conn = this.Configuration.GetConnectionString("myconn");
            using (SQLiteConnection db = new SQLiteConnection(Conn))
            {
                ViewBag.ptName = ptName;
                ViewBag.cl = db.Query<Doctor>("Select * from Doctor ").ToList();
                ViewBag.ct = db.Query<City>("Select * from City ").ToList();
                ViewBag.st = db.Query<State>("Select * from State ").ToList();
                ViewBag.cs = db.Query<Disease>("Select * from Disease ").ToList();
                patients = db.Query<Patient>("select Patient.*,City.cityName as cityName ,Disease.diseaseName as diseaseName ,State.stateName as stateName ,Doctor.doctorName as doctorName " +
                    "from Patient left join City on Patient.cityId=City.id left join Disease on Patient.diseaseId=Disease.id left join State on Patient.stateId=State.id left join Doctor on Patient.doctorId=Doctor.id where " + where).ToList();
            }
            decimal totalFees = patients.Sum(s => s.fees);
            DataTable dt = new DataTable("Patient");
            dt.Columns.AddRange(new DataColumn[9]
                {
                     new DataColumn("Name"),
                     new DataColumn("Doctor"),
                     new DataColumn("City"),
                     new DataColumn("State"),
                     new DataColumn("Disease"),
                     new DataColumn("Caste"),
                     new DataColumn("Gender"),
                     new DataColumn("Fees"),
                     new DataColumn("Mob No"),
            });
            foreach (Patient obj in patients)
            {
                dt.Rows.Add(obj.ptName,
                    obj.doctorName,
                    obj.cityName,
                    obj.stateName,
                    obj.diseaseName,
                    obj.cast,
                    obj.gender == "1" ? "Male" : obj.gender == "2" ? "Female" : "",
                    obj.fees,
                    obj.mobNo

                    );
            }
            dt.Rows.Add("Total Fees", "", "", "", "", "", "", totalFees, "");
            using (XLWorkbook wb = new XLWorkbook())

            {
                IXLWorksheet ws = wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);

                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Patient.xlsx");
                }

            }
        }

        //pdf download code 
        public FileContentResult PdfExport(string ptName, int cityId, int doctorId, int stateId, int diseaseId)
        {
            string where = " 1=1";
            if (ptName != null)
            {   
                where += " and Patient.ptName='" + ptName + "'";
            }
            if (cityId != 0)
            {
                where += " and Patient.cityId=" + cityId;
            }
            if (doctorId != 0)
            {
                where += " and Patient.doctorId=" + doctorId;
            }
            if (stateId != 0)
            {
                where += " and Patient.stateId=" + stateId;
            }
            if (diseaseId != 0)
            {
                where += " and Patient.diseaseId=" + diseaseId;
            }

            List<Patient> patients = new List<Patient>();
            string city ;
            string classes ;
            string state;
            string disease;
            string Conn = this.Configuration.GetConnectionString("myconn");
            using (SQLiteConnection db = new SQLiteConnection(Conn))
            {

                ViewBag.ptName = ptName;
                ViewBag.cl = db.Query<Doctor>("Select * from Doctor ").ToList();
                ViewBag.ct = db.Query<City>("Select * from City ").ToList();
                ViewBag.st = db.Query<State>("Select * from State ").ToList();
                ViewBag.cs = db.Query<Disease>("Select * from Disease ").ToList();
                patients = db.Query<Patient>("select Patient.*,City.cityName as cityName ,Disease.diseaseName as diseaseName ,State.stateName as stateName ,Doctor.doctorName as doctorName " +
                    "from Patient left join City on Patient.cityId=City.id left join Disease on Patient.diseaseId=Disease.id left join State on Patient.stateId=State.id left join Doctor on Patient.doctorId=Doctor.id where " + where).ToList();
               city     = db.ExecuteScalar<string>("select cityName from City where id="+ cityId);
               classes  = db.ExecuteScalar<string>("select doctorName from Doctor where id=" + doctorId);
               state   =  db.ExecuteScalar<string>("select stateName from State where id=" + stateId);
               disease = db.ExecuteScalar<string>("select diseaseName from Disease where id=" + diseaseId);
            }
            decimal totalFees = patients.Sum(s => s.fees);
            iTextSharp.text.Font fonta = FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font fontb = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font fontc = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font fontd = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

            MemoryStream mmStream = new MemoryStream();
            Document doc = new Document(PageSize.A4, 15, 15, 15, 15);

            PdfWriter pdfWriter = PdfWriter.GetInstance(doc, mmStream);
            doc.Open();
            PdfContentByte cb = pdfWriter.DirectContent;

            iTextSharp.text.Paragraph report = new iTextSharp.text.Paragraph("Patients List", fontd);
            report.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            report.Font = fontd;
            doc.Add(report);

            PdfPTable table1 = new PdfPTable(4);
            float[] widths1 = new float[] { 1.4f, 1.4f, 1.4f, 1.4f};
            table1.SetWidths(widths1);
            table1.SpacingBefore = 20;
            table1.TotalWidth = 560;
            table1.LockedWidth = true;
            PdfPCell cell1;

            if (cityId > 0)
            {
                cell1 = new PdfPCell(new Phrase("City : " + city, fontd));
                cell1.HorizontalAlignment = 1;
                table1.AddCell(cell1);
            }
            else
            {
                cell1 = new PdfPCell(new Phrase("City : ", fontd));
                cell1.HorizontalAlignment = 1;
                table1.AddCell(cell1);
            }
            if (diseaseId > 0)
            {
                cell1 = new PdfPCell(new Phrase("Disease : " + disease, fontd));
                cell1.HorizontalAlignment = 1;
                table1.AddCell(cell1);
            }
            else
            {
                cell1 = new PdfPCell(new Phrase("Disease : ", fontd));
                cell1.HorizontalAlignment = 1;
                table1.AddCell(cell1);
            }
            if (doctorId > 0)
            {
                cell1 = new PdfPCell(new Phrase("Doctor : " + classes, fontd));
                cell1.HorizontalAlignment = 1;
                table1.AddCell(cell1);
            }
            else
            {
                cell1 = new PdfPCell(new Phrase("Doctor : ", fontd));
                cell1.HorizontalAlignment = 1;
                table1.AddCell(cell1);
            }
            if (stateId > 0)
            {
                cell1 = new PdfPCell(new Phrase("State : " + state, fontd));
                cell1.HorizontalAlignment = 1;
                table1.AddCell(cell1);
            }
            else
            {
                cell1 = new PdfPCell(new Phrase("State : ", fontd));
                cell1.HorizontalAlignment = 1;
                table1.AddCell(cell1);
            }

            
            doc.Add(table1);

            PdfPTable table = new PdfPTable(10);
            float[] widths = new float[] { .7f, .7f, .6f, .9f, .6f,.7f,.7f,.7f,.6f,.8f };
            table.SetWidths(widths);
            table.SpacingBefore = 20;
            table.TotalWidth = 560;
            table.LockedWidth = true;

            PdfPCell cell;
            cell = new PdfPCell(new Phrase("Patient Image", fontd)); 
            cell.HorizontalAlignment = 1;
            table.AddCell(cell); 
            
            cell = new PdfPCell(new Phrase("Patient Name", fontd));
            cell.HorizontalAlignment = 1;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Doctor", fontd));
            cell.HorizontalAlignment = 1;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("City", fontd));
            cell.HorizontalAlignment = 1;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("State", fontd));
            cell.HorizontalAlignment = 1;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("Disease", fontd));
            cell.HorizontalAlignment = 1;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Caste", fontd));
            cell.HorizontalAlignment = 1;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Gender", fontd));
            cell.HorizontalAlignment = 1;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("Fees", fontd));
            cell.HorizontalAlignment = 1;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("Mob No", fontd));
            cell.HorizontalAlignment = 1;
            table.AddCell(cell);


            foreach (Patient obj in patients)
            {

                string imagePath = Path.Combine(Environment.WebRootPath, obj.ImagePath.TrimStart('/'));
                byte[] imageData = System.IO.File.ReadAllBytes(imagePath);

                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imageData);
                image.ScaleAbsolute(50f, 50f); 
                cell = new PdfPCell(image);
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(obj.ptName, fontb));
                cell.HorizontalAlignment = 0;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(obj.doctorName, fontb));
                cell.HorizontalAlignment = 0;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(obj.cityName, fontb));
                cell.HorizontalAlignment = 0;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(obj.stateName, fontb));
                cell.HorizontalAlignment = 0;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(obj.diseaseName, fontb));
                cell.HorizontalAlignment = 0;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(obj.cast, fontb));
                cell.HorizontalAlignment = 0;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(obj.gender == "1" ? "Male" : obj.gender == "2" ? "Female" : "", fontb));
                cell.HorizontalAlignment = 0;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(obj.fees.ToString(), fontb));
                cell.HorizontalAlignment = 2;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(obj.mobNo, fontb));
                cell.HorizontalAlignment = 2;
                table.AddCell(cell);


            }
            cell = new PdfPCell(new Phrase("Total Fees ", fontd));
            cell.Colspan = 8; 
            cell.HorizontalAlignment = 2;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(totalFees.ToString(), fontb));
            cell.HorizontalAlignment = 2;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("", fontb));
            cell.HorizontalAlignment = 1;
            table.AddCell(cell);

            doc.Add(table);
            pdfWriter.CloseStream = false;
            doc.Close();

            byte[] bytea = mmStream.ToArray();
            return File(bytea, "application/pdf", "Patients.pdf");
        }

    }
}

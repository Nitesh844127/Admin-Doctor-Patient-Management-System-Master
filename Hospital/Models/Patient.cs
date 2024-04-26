namespace Hospital.Models
{
    public class Patient
    {
        public int id { get; set; }
        public string ptName { get; set; }
        public int doctorId { get; set;}
        public int cityId { get; set; }
        public int stateId { get; set; }
        public int diseaseId { get; set; }
        public string cast { get; set; }
        public string gender { get; set; }
        public decimal fees { get; set; }
        public decimal dueFees { get; set; }
        public string mobNo { get; set; }
        public string cityName { get; set; }
        public string diseaseName { get; set; }
        public string doctorName { get; set; }
        public string stateName { get; set; }
        public string ImagePath { get; set; }

    }
}

namespace Hospital.Models
{
    public class Receipt
    {
        public int id { get; set; }
        public int date { get; set; } 
        public int ptId { get; set; } 
        public decimal amount { get; set; } 
        public string remark { get; set; }
        public string patName { get; set; }
        public string diseaseName { get; set; }
        public string doctorName { get; set;}
        public string mob { get; set;}


    }
}


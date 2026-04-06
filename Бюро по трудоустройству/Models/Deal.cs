using System;

namespace Бюро_по_трудоустройству.Models
{
    public class Deal
    {
        public int Id { get; set; }
        public Employer Employer { get; set; }
        public JobSeeker JobSeeker { get; set; }
        public string Position { get; set; }
        public decimal Commission { get; set; }
        public DateTime Date { get; set; }
    }
}
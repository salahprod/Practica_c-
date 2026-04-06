using System;

namespace Бюро_по_трудоустройству.Models
{
    public class JobSeeker
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Patronymic { get; set; }
        public string Qualification { get; set; }
        public string ActivityType { get; set; }
        public string OtherInfo { get; set; }
        public decimal ExpectedSalary { get; set; }

        public string FullName => $"{LastName} {FirstName} {Patronymic}".Trim();
    }
}
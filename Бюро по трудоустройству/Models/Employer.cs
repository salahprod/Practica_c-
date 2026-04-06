using System;

namespace Бюро_по_трудоустройству.Models
{
    public class Employer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ActivityType { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }

        public override string ToString() => Name;
    }
}
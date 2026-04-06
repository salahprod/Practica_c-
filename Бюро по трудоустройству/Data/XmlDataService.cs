using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using Бюро_по_трудоустройству.Models;
using System;

namespace Бюро_по_трудоустройству.Data
{
    public class XmlDataService
    {
        private string employersFile = "employers.xml";
        private string seekersFile = "seekers.xml";
        private string dealsFile = "deals.xml";

        public List<Employer> LoadEmployers()
        {
            if (!File.Exists(employersFile))
                return new List<Employer>();

            try
            {
                var doc = XDocument.Load(employersFile);
                return doc.Root.Elements("Employer").Select(e => new Employer
                {
                    Id = (int)e.Element("Id"),
                    Name = (string)e.Element("Name"),
                    ActivityType = (string)e.Element("ActivityType"),
                    Address = (string)e.Element("Address"),
                    Phone = (string)e.Element("Phone")
                }).ToList();
            }
            catch
            {
                return new List<Employer>();
            }
        }

        public void SaveEmployers(List<Employer> employers)
        {
            var doc = new XDocument(
                new XElement("Employers",
                    employers.Select(e => new XElement("Employer",
                        new XElement("Id", e.Id),
                        new XElement("Name", e.Name ?? ""),
                        new XElement("ActivityType", e.ActivityType ?? ""),
                        new XElement("Address", e.Address ?? ""),
                        new XElement("Phone", e.Phone ?? "")
                    ))
                )
            );
            doc.Save(employersFile);
        }

        public List<JobSeeker> LoadSeekers()
        {
            if (!File.Exists(seekersFile))
                return new List<JobSeeker>();

            try
            {
                var doc = XDocument.Load(seekersFile);
                return doc.Root.Elements("Seeker").Select(s => new JobSeeker
                {
                    Id = (int)s.Element("Id"),
                    LastName = (string)s.Element("LastName"),
                    FirstName = (string)s.Element("FirstName"),
                    Patronymic = (string)s.Element("Patronymic"),
                    Qualification = (string)s.Element("Qualification"),
                    ActivityType = (string)s.Element("ActivityType"),
                    OtherInfo = (string)s.Element("OtherInfo"),
                    ExpectedSalary = (decimal)s.Element("ExpectedSalary")
                }).ToList();
            }
            catch
            {
                return new List<JobSeeker>();
            }
        }

        public void SaveSeekers(List<JobSeeker> seekers)
        {
            var doc = new XDocument(
                new XElement("Seekers",
                    seekers.Select(s => new XElement("Seeker",
                        new XElement("Id", s.Id),
                        new XElement("LastName", s.LastName ?? ""),
                        new XElement("FirstName", s.FirstName ?? ""),
                        new XElement("Patronymic", s.Patronymic ?? ""),
                        new XElement("Qualification", s.Qualification ?? ""),
                        new XElement("ActivityType", s.ActivityType ?? ""),
                        new XElement("OtherInfo", s.OtherInfo ?? ""),
                        new XElement("ExpectedSalary", s.ExpectedSalary)
                    ))
                )
            );
            doc.Save(seekersFile);
        }

        public List<Deal> LoadDeals(List<Employer> employers, List<JobSeeker> seekers)
        {
            if (!File.Exists(dealsFile))
                return new List<Deal>();

            try
            {
                var doc = XDocument.Load(dealsFile);
                return doc.Root.Elements("Deal").Select(d => new Deal
                {
                    Id = (int)d.Element("Id"),
                    Employer = employers.FirstOrDefault(e => e.Id == (int)d.Element("EmployerId")),
                    JobSeeker = seekers.FirstOrDefault(s => s.Id == (int)d.Element("SeekerId")),
                    Position = (string)d.Element("Position"),
                    Commission = (decimal)d.Element("Commission"),
                    Date = (DateTime)d.Element("Date")
                }).Where(d => d.Employer != null && d.JobSeeker != null).ToList();
            }
            catch
            {
                return new List<Deal>();
            }
        }

        public void SaveDeals(List<Deal> deals)
        {
            var doc = new XDocument(
                new XElement("Deals",
                    deals.Select(d => new XElement("Deal",
                        new XElement("Id", d.Id),
                        new XElement("EmployerId", d.Employer.Id),
                        new XElement("SeekerId", d.JobSeeker.Id),
                        new XElement("Position", d.Position ?? ""),
                        new XElement("Commission", d.Commission),
                        new XElement("Date", d.Date)
                    ))
                )
            );
            doc.Save(dealsFile);
        }
    }
}